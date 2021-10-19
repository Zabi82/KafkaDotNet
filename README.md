# Kafka Dotnet Client Lab

## Lab exercise using Kafka Client for .NET on Ubuntu OS

## Prerequisites 

### Install OpenJDK 11

To install OpenJDK11, first update the package index

```
sudo apt update
```

Next, install default Java OpenJDK package with

```
sudo apt install default-jdk
```

Verify the java installation

```
java -version
```

The output will be something similar like this

```
openjdk version "11.0.2" 2019-01-15
OpenJDK Runtime Environment (build 11.0.2+9-Ubuntu-3ubuntu118.04.3)
OpenJDK 64-Bit Server VM (build 11.0.2+9-Ubuntu-3ubuntu118.04.3, mixed mode, sharing)
```

### Install Kafka

#### Step 1 - Creating a User for Kafka

Create a user called kafka:

```
sudo useradd kafka -m
```

Set the password using `passwd`:

```
sudo passwd kafka
```

Add the kafka user to the `sudo` group with the `adduser` command

```
sudo adduser kafka sudo
```

#### Step 2 - Downloading & Extracting the Kafka Binaries

Create a directory in `/home/kafka/` called `downloads` to keep your downloads

```
mkdir /home/kafka/downloads
```

Use `curl` to download the Kafka binaries:

```
curl "https://archive.apache.org/dist/kafka/2.8.0/kafka_2.13-2.8.0.tgz" -o /home/kafka/downloads/kafka.tgz
```

Create a directory named `kafka` and change to this directory. This will be the base directory for kafka

```
mkdir /home/kafka/kafka && cd /home/kafka/kafka
```

Extract the downloaded archive using `tar` command:

```
tar -xvzf /home/kafka/downloads/kafka.tgz --strip 1
```

### Install VSCode

Update the package indexes and install the dependencies by running the following command:

```
sudo apt update
sudo apt install software-properties-common apt-transport-https wget
```

Import the Microsoft GPG key and enable Visual Studio Code repository

```
wget -q https://packages.microsoft.com/keys/microsoft.asc -O- | sudo apt-key add -
sudo add-apt-repository "deb [arch=amd64] https://packages.microsoft.com/repos/vscode stable main"
```

Install Visual Studio Code package

```
sudo apt install code
```

### Install Dotnet Core

.NET Core SDK is the Software Development Kit used for developing applications. To install .NET Core on Ubuntu, run the following commands:

```
sudo apt update
sudo apt install apt-transport-https
sudo apt install dotnet-sdk-3.1
```

Check .NET Core version

```
dotnet --version
```

#### Configure JAVA_HOME

To create JAVA_HOME environment variable, open `/etc/environment` file:

```
sudo nano /etc/environment
```

Add the following line at the end of the file:

```
JAVA_HOME="/usr/lib/jvm/java-11-open-jdk-amd64"
```

#### Start Zookeeper & Kafka

To start Zookeeper, run the following command:

```
/home/kafka/kafka/bin/zookeeper-server-start.sh /home/kafka/kafka/config/zookeeper.properties
```

Post successful starting of Zookeerp, run following command to start Kafka cluster

```
/home/kafka/kafka/bin/kafka-server-start.sh /home/kafka/kafka/config/server.properties > /home/kafka/kafka/kafka.log 2>&1
```

#### C# support on Visual Studio Code

Refer this url `https://code.visualstudio.com/docs/languages/csharp` to enable C# language support on Visual Studio Code

## Run the program

### Producer

Open terminal, go to project folder and run

```
dotnet run producer <name of topic>
```

### Consumer

Open terminal, go to project folder  and run

```
dotnet run consumer <name of topic>
```
