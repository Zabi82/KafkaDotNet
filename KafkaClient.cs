using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaDotnetClient {
    class KafkaClient {

        const string BOOTSTRAP_SERVERS = "localhost:9092";
        const int NUM_MESSAGES = 10;
        const int TOTAL_PARTITIONS = 1;
        const int TOTAL_REPLICATIONS = 1;

        const String CONSUMER_GROUP_ID = "testgroup_1";

        static async Task CreateTopic(string name, ClientConfig config) {
            using (var adminClient = new AdminClientBuilder(config).Build()) {
                try {
                    await adminClient.CreateTopicsAsync(new List<TopicSpecification> {
                        new TopicSpecification { Name = name, NumPartitions = TOTAL_PARTITIONS, ReplicationFactor = TOTAL_REPLICATIONS } });
                } catch (CreateTopicsException e) {
                    if (e.Results[0].Error.Code != ErrorCode.TopicAlreadyExists) {
                        Console.WriteLine($"An error occured creating topic {name}: {e.Results[0].Error.Reason}");
                    } else {
                        Console.WriteLine("Topic already exists");
                    }
                }
            }
        }
        
        static void Produce(string topic, ClientConfig config) {
            using (var producer = new ProducerBuilder<string, string>(config).Build()) {
                int numProduced = 0;
                for (int i=0; i<NUM_MESSAGES; ++i) {
                    var key = "key " + i;
                    var val =  "value " + i; //JObject.FromObject(new { count = i }).ToString(Formatting.None);

                    Console.WriteLine($"Producing record: {key} {val}");

                    producer.Produce(topic, new Message<string, string> { Key = key, Value = val },
                        (deliveryReport) => {
                            if (deliveryReport.Error.Code != ErrorCode.NoError) {
                                Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                            } else {
                                Console.WriteLine($"Produced message to: {deliveryReport.TopicPartitionOffset}");
                                numProduced += 1;
                            }
                        });
                }

                producer.Flush(TimeSpan.FromSeconds(10));

                Console.WriteLine($"{numProduced} messages were produced to topic {topic}");
            }
        }

        static void Consume(string topic, ClientConfig config) {
            var consumerConfig = new ConsumerConfig(config);
            consumerConfig.GroupId = CONSUMER_GROUP_ID;
            consumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
            consumerConfig.EnableAutoCommit = true;

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true;
                cts.Cancel();
            };

            using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build()) {
                consumer.Subscribe(topic);
                try {
                    while (true) {
                        var cr = consumer.Consume(cts.Token);
                        Console.WriteLine($"Consumed record with key {cr.Message.Key} and value {cr.Message.Value}");
                    }
                } catch (OperationCanceledException) {
                    // Ctrl-C was pressed.
                } finally {
                    consumer.Close();
                }
            }
        }

        static void PrintUsage() {
            Console.WriteLine("Usage: dotnet run <producer (OR) consumer> <topic>");
            System.Environment.Exit(1);
        }

        static async Task Main(string[] args) {
            if (args.Length != 2) { 
                PrintUsage(); 
            }
            
            var mode = args[0];
            var topic = args[1];

            var config = new ClientConfig(new Dictionary<string, string>() { {"bootstrap.servers", BOOTSTRAP_SERVERS} });

            switch (mode) {
                case "producer":
                    await CreateTopic(topic, config);
                    Produce(topic, config);
                    break;
                case "consumer":
                    Consume(topic, config);
                    break;
                default:
                    PrintUsage();
                    break;
            }
        }
    }
}
