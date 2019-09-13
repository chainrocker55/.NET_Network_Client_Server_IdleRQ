using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
namespace Face_Detect_AI
{
    class Program
    {
        const string Endpoint = "https://southcentralus.api.cognitive.microsoft.com/";
        const string TrainingKey = "9021604613644309908201018ab0cca8";
        const string PredictionKey = "b93bee263ed54fceb64355686fbe0bb3";
        const string PredictionResourceId = "/subscriptions/0a088922-63b8-498b-9fc2-f91778aee872/resourceGroups/AI/providers/Microsoft.CognitiveServices/accounts/Predict-face";

        static void Main(string[] args)
        {
            var trainingClient = new CustomVisionTrainingClient()
            {
                ApiKey = TrainingKey,
                Endpoint = Endpoint
            };

            Console.WriteLine("Creating new project:");
      
            var project = trainingClient.CreateProject("Face-Detect");         

            Console.WriteLine("Uploading Prayut images.");
            var prayutTag = trainingClient.CreateTag(project.Id, "Prayut");
            var prayutImages = Directory.GetFiles(Path.Combine("Images", "Prayut"));
            var prayutFiles = prayutImages.Select(img => new ImageFileCreateEntry(Path.GetFileName(img), File.ReadAllBytes(img))).ToList();
            trainingClient.CreateImagesFromFiles(project.Id, new ImageFileCreateBatch(prayutFiles, new List<Guid>() { prayutTag.Id }));
            Console.WriteLine("-> Done.");

            Console.WriteLine("Uploading Thaksin images.");
            var thaksinTag = trainingClient.CreateTag(project.Id, "Thaksin");
            var thaksinImages = Directory.GetFiles(Path.Combine("Images", "thaksin"));
            var thaksinFiles = thaksinImages.Select(img => new ImageFileCreateEntry(Path.GetFileName(img), File.ReadAllBytes(img))).ToList();
            trainingClient.CreateImagesFromFiles(project.Id, new ImageFileCreateBatch(thaksinFiles, new List<Guid>() { thaksinTag.Id }));
            Console.WriteLine("-> Done.");

            Console.WriteLine("Training.");
            var iteration = trainingClient.TrainProject(project.Id);
            while (iteration.Status == "Training")
            {
                Thread.Sleep(1000);
                iteration = trainingClient.GetIteration(project.Id, iteration.Id);
            }
            Console.WriteLine("-> Done.");

            Console.WriteLine("Publishing.");
            var publishedName = "GuessWho";
            trainingClient.PublishIteration(project.Id, iteration.Id, publishedName, PredictionResourceId);
            Console.WriteLine("-> Done.");

            Console.WriteLine("Making a prediction.");
            var predictionClient = new CustomVisionPredictionClient()
            {
                ApiKey = PredictionKey,
                Endpoint = Endpoint
            };
            var testImage = new MemoryStream(File.ReadAllBytes(Path.Combine("Images", @"Test\Test.jpg")));
            var result = predictionClient.ClassifyImage(project.Id, publishedName, testImage);
            foreach (var prediction in result.Predictions)
            {
                Console.WriteLine($"-> {prediction.TagName}: {prediction.Probability:P1}");
            }
            Console.WriteLine("-> Done.");

            Console.WriteLine("Deleting your project.");
            trainingClient.UnpublishIteration(project.Id, iteration.Id);
            trainingClient.DeleteIteration(project.Id, iteration.Id);
            trainingClient.DeleteProject(project.Id);
            Console.WriteLine("-> Done.");

        }
    }
}
