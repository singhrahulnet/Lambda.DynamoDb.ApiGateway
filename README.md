# Lambda.DynamoDB.ApiGateway
The project has been created using AWS serverless stack. It consists of - 
* Two Lambdas named `StartMining` and `GetMiningStatus`
* The Lambdas are behind an API Gateway (POST and GET)
* A DynamoDB table to store and retrieve data
* AWS CloudFormation SAM model used for Infra as Code
* Unit and Integration tests


## Setup

Download and install `.NET Core 3.1` and IDE of your choice.

Install `Amazon.Lambda.Tools` Global tools if not already installed.
```
    dotnet tool install -g Amazon.Lambda.Tools
```

If it is already installed check if new version is available.
```
    dotnet tool update -g Amazon.Lambda.Tools
```

## Running Unit Tests
Open `test/ILV.Api.Tests` folder in terminal and run the following command -
```
    dotnet test --filter Unit
```

## Running Integration Tests
* Note that integration tests will take a bit longer to run as some tests are written to simulate up to 90 seconds of delay before making an Api call. 
* The tests create a temporary DynamoDB table as well and delete it after execution.
* Make sure that an AWS account is _configured_ in your IDE.

Open `test/ILV.Api.Tests` folder in terminal and run the following command
```
    dotnet test --filter Integration
```

## Deploying the Stack
* Review the AWS settings are defined in [`aws-lambda-tools-defaults.json`](src/ILV.Api/aws-lambda-tools-defaults.json) file. 
* Couldn't get SAM template to create S3 bucket (for uploading artifacts) with first time deployment. It could be access issue or a bug. You may have to create (just once) a S3 bucket named `ilv-api-bundle"` as defined the the file above, sorry! 

Open `src/ILV.Api` folder in terminal and run the following command -
```
    dotnet lambda deploy-serverless
```
The command will create necessary resources for the app and an API should be ready to use via the API Gateway Service.

## TODOs

* Couldn't get the CloudFormation to output the Api endpoint yet. It would be neat to test the endpoints using curl right there in the IDE rather than having to look for the URL in the AWS Console.
