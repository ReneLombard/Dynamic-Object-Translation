# Dynamic-Object-Translation
A library built on Azure Translation Services that makes it easier to translate serialized objects.


## Introduction
This library provides a means that you can easily integrate data translations mechanism to your .NET core applications.

It is built on the [Azure Congnitive Services](https://azure.microsoft.com/en-us/services/cognitive-services/translator/#features)


It makes it possible to translate a given serializable complex object and create a new object with the original complex type but translated. 

## Prerequisites
You will require a Azure Subscription Key to get up and running.
"Ocp-Apim-Subscription-Key"

## Getting Started
In addition to this getting started tutorial, you can find a sample application in the solution, which would demonstrated the capabilities of this library. 

## Installation
Add the following to your service collection. 
`services.Configure<AzureConfig>(Configuration.GetSection("Azure"));
services.AddTranslation<BlogPost>();`
Where the BlogPost refers to your model class that you want to translate. 

In you configuration file add the following section 
`"Azure": {
    "TranslationSubscriptionKey": "XXXXXXXXXX"
  }`
XXXXXXXXXX refers Azure Subscription Key 

In your model class please make sure that you have the `[Serializable]` attribute above your class.

For the class that will be utilizing the translation Service add the following to the constructor
`TranslationService<BlogPost> translationService` 

and execute the translation as follows
`var translatedObject = await translationService.TranslateAsync(yourObject, "nl") `
Where the "nl" refers to the destination language that will be sent to the Azure Cognitive Services 

### Additional Resources
Please see a Sample application in ReneLombard.Translation.Sample folder

For the sample application please just set the TranslationSubscriptionKey in the appSettings.json

The sample application uses random sentences provided by the AutoBogus (2.8.2) library. These sentences are translated to Dutch sentences and populates a BlogPost Object.

## Versioning
We use [SemVer](https://semver.org/) for versioning. For the versions available, see the tags on this repository.
