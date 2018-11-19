using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

using System;
using System.IO;
using System.Threading.Tasks;

namespace ExtractText
{
    class Program
    {
        // define subscription key.  You will need to get this key from the cognitive scale azure instance.  Key below is from my private azure instance
        // The target API endpoint must match the region of the cognitive scale subscription.  set the following variable in the code below accordingly, e.g. computerVision.Endpoint = "https://westeurope.api.cognitive.microsoft.com";

        private const string subscriptionKey = "2271609594c2427797f4eb7fdaf3d70d";

        private const TextRecognitionMode textRecognitionMode =
            TextRecognitionMode.Handwritten;

        
        private const string remoteImageUrl =
            "https://upload.wikimedia.org/wikipedia/commons/thumb/d/dd/" +
            "Cursive_Writing_on_Notebook_paper.jpg/" +
            "800px-Cursive_Writing_on_Notebook_paper.png";

        private const int numberOfCharsInOperationId = 36;

        static void Main(string[] args)
        {
            //instantiate computervisionclient from vision SDK and bind creds using subscription key
            ComputerVisionClient computerVision = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(subscriptionKey),
                new System.Net.Http.DelegatingHandler[] { });

            Console.WriteLine("This command line tool will extract hand written text from " +
                "images \n  \n The tool will default to hardwritten URI for a test image.   \n  However, it will allow you to manually enter a local image for processing \n");
            Console.WriteLine("It will process images using the Computer Vision Windows client library which is calling Cognitive Services API from MS Azure.\n");
            Console.WriteLine(
                    "\nPlease enter full pathname for local image to process, e.g. C:\\x\\cognitivescale.png   (Then press ENTER) :");
            String localImagePath = Console.ReadLine();

            //change this endpoint to match the subscription region used by cognitivescale
            computerVision.Endpoint = "https://westeurope.api.cognitive.microsoft.com";

            //use var to house instantiation of both tasks asynchronously
            Console.WriteLine("Images being analyzed ...");
            var t1 = ExtractRemoteTextAsync(computerVision, remoteImageUrl);
            var t2 = ExtractLocalTextAsync(computerVision, localImagePath);
            
            //awaiting async tasks to complete
            Task.WhenAll(t1, t2).Wait(5000);
            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
            //end
        }

        // Recognize text from a remote image
        private static async Task ExtractRemoteTextAsync(
            ComputerVisionClient computerVision, string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                Console.WriteLine(
                    "\nInvalid remoteImageUrl:\n{0} \n", imageUrl);
                return;
            }

            // Start the async process to recognize the text and hold (wait) for execution to complete
            RecognizeTextHeaders textHeaders =
                await computerVision.RecognizeTextAsync(
                    imageUrl, textRecognitionMode);
            //start gettext and await completion
            await GetTextAsync(computerVision, textHeaders.OperationLocation);
        }



        // thread to Recognize text from a local image
        private static async Task ExtractLocalTextAsync(
            ComputerVisionClient computerVision, string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                Console.WriteLine(
                    "\nUnable to open or read localImagePath:\n{0} \n", imagePath);
                return;
            }

            using (Stream imageStream = File.OpenRead(imagePath))
            {
                // Start the async process to recognize the text
                RecognizeTextInStreamHeaders textHeaders =
                    await computerVision.RecognizeTextInStreamAsync(
                        imageStream, textRecognitionMode);

                await GetTextAsync(computerVision, textHeaders.OperationLocation);
            }
        }


        // method to retrieve the recognized text
        private static async Task GetTextAsync(
            ComputerVisionClient computerVision, string operationLocation)
        {
            // Retrieve the URI where the recognized text will be
            // stored from the Operation-Location header
            string operationId = operationLocation.Substring(
                operationLocation.Length - numberOfCharsInOperationId);

            Console.WriteLine("\nCalling GetHandwritingRecognitionOperationResultAsync()");
            TextOperationResult result =
                await computerVision.GetTextOperationResultAsync(operationId);

            // Wait for the operation to complete
            int i = 0;
            int maxRetries = 10;
            while ((result.Status == TextOperationStatusCodes.Running ||
                    result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
            {
                Console.WriteLine(
                    "Server status: {0}, waiting {1} seconds...", result.Status, i);
                await Task.Delay(1000);

                result = await computerVision.GetTextOperationResultAsync(operationId);
            }

            // Display the results
            Console.WriteLine();
            var lines = result.RecognitionResult.Lines;
            foreach (Line line in lines)
            {
                Console.WriteLine(line.Text);
            }
            Console.WriteLine();
            Console.WriteLine("EOF");
        }
    }
}