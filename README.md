# Translator (Public)

Translate from multiple languages to English using Azure Speech Service. The app will produce an output WAV in English plus the translated text

This is a basic console app created using .Net5 and requires an instance of Azure Speech Service. The required Key can then be used from your Speech Service instance to run the cognitive services translation.

![image](https://user-images.githubusercontent.com/28670731/189649496-6efaaf5c-a112-4cbd-8085-c952d1514b50.png)

**List of supported languages:**
https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#neural-voices

**Note:** You may need to use something like Audacity (https://www.audacityteam.org) to export the WAV file as 16bit PCM if the translation fails.

## TODO

Look at Azure Key Vault for storage of sensitive key as its currently too easy to get access to it. 

This app could do with a bit of refactoring but I might look at creating a Blazor WASM version with access to Azure Storage via a Function App. Azure AD auth will be needed to ensure the resource is not abused.

Output translated text and WAV files to a storage account.

Look at integrating FFMPEG to ensure input file is in a usable format.
