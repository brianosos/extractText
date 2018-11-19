# extractText
  
This app leverages 'Microsoft.Azure.CognitiveServices.Vision.ComputerVision' .net libraries to connect and leverage API from 
MS Azure Cognitive Services in order to Extract text from graphical images.


Notes
  The application demonstrates extraction of text from images stored at both a hardwired URL & from user entered local filename 
  LOCAL IMAGES Requires full path, e.g. c:\files\cognitivescale.png)
  REMOTE IMAGES Hardwired URL is -> https://upload.wikimedia.org/wikipedia/commons/thumb/d/dd/Cursive_Writing_on_Notebook_paper.jpg/800px-Cursive_Writing_on_Notebook_paper.png";



Install instructions

  Simply download visual studio soluion '.sln' and open into & compile in Visual Studio.

  Run the console app exe within Visual Studio, or run the exe direct from command line.


Pre-requisites 

  Developed in VS 2017.   .NET version xyz

  Requires a subscription key from an Azure Cognitive Services.  Current Key hardwired in the app is from my personal Azure instance.  You 
  will need to extract one from an Azure subscription with instance of Cognitive Services. 
  The app is currently hardwired to connect to western europe region of MS Azure.  If your Azure instance is elsewhere, you will need to 
  change the URI config.  
  
  e.g. console app is currently hardwired to "https://westeurope.api.cognitive.microsoft.com".   For WestUS, then "https://westus.api.cognitive.microsoft.com/"
  Note that the target API URI can be found in the overview of the subscription in Azure.
  
  A demo image 'cognitivescale.png'is included with github repo.
