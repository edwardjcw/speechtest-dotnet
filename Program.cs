using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace SynthesizeVoice
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            var testRequest = SynthesizeRequest.CreateMinimalRequest("Hello sharp world");
            var responseResult = ProcessRequest(testRequest, args[0]).Result;
            var b = Convert.FromBase64String(responseResult.AudioContent);
            System.IO.File.WriteAllBytes(@"outtest.wav", b);
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
            // var url = String.Format("https://texttospeech.googleapis.com/v1beta1/text:synthesize?key={}", apiKey);
            var url = String.Format("http://127.0.0.1:3000/v1beta1/text:synthesize?key={0}", apiKey);
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