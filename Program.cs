using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace SynthesizeVoice
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "speechtest-dotnet";
            app.Description = ".NET Core console app that transforms text to speech using Google's API.";
            app.HelpOption("-?|-h|--help");
            app.VersionOption("-v|--version", () => {
                return string.Format("Version {0}", Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
            });

            var keyOption = app.Option("-k|--key <APIKEY>", 
                    "The Google Cloud API key to use for authentication",
                    CommandOptionType.SingleValue);
            var inputOption = app.Option("-i|--input <TEXT>", 
                    "The text you want to convert to speech",
                    CommandOptionType.SingleValue);
            var outputOption = app.Option("-o| --output <PATH>", 
                    "Path and name of the output wav. Defaults to ./output.wav",
                    CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                if (keyOption.HasValue() && inputOption.HasValue())
                {
                    var testRequest = SynthesizeRequest.CreateMinimalRequest(inputOption.Value());
                    var responseResult = ProcessRequest(testRequest, keyOption.Value()).Result;
                    var b = Convert.FromBase64String(responseResult.AudioContent);
                    var output = outputOption.Value() ?? @"outtest.wav";
                    System.IO.File.WriteAllBytes(output, b);
                }
                else
                {
                    Console.WriteLine("No options specified.");
                    app.ShowHint();
                }

                return 0;
            });

            try
            {
                app.Execute(args);
            }
            catch (CommandParsingException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to execute application: {0}", ex.Message);
            }
        }

        private static string CreateRequestBody(SynthesizeRequest request)
        {
            var requestSerializer = new DataContractJsonSerializer(typeof(SynthesizeRequest));
            var ms = new MemoryStream();
            requestSerializer.WriteObject(ms, request);
            var json = ms.ToArray();
            ms.Dispose();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        private static async Task<SynthesizeResponse> ProcessRequest(SynthesizeRequest request, string apiKey)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var responseSerializer = new DataContractJsonSerializer(typeof(SynthesizeResponse));
            client.DefaultRequestHeaders.Accept.Clear();
            var url = String.Format("https://texttospeech.googleapis.com/v1beta1/text:synthesize?key={0}", apiKey);
            var postResponse = client
                .PostAsync(
                    url,
                    new StringContent(CreateRequestBody(request), Encoding.ASCII, "application/json")
                )
                .Result
                .Content
                .ReadAsStreamAsync();
            return responseSerializer
                .ReadObject(await postResponse) as SynthesizeResponse;
        }
    }
}
