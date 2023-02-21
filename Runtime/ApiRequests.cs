using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace PixelmindSDK
{
    public class ApiRequests
    {
        public static async Task<List<Generator>> GetGenerators(string apiKey)
        {
            var getGeneratorsRequest = UnityWebRequest.Get(
                "https://backend.blockadelabs.com/api/v1/generators" + "?api_key=" + apiKey
            );

            await getGeneratorsRequest.SendWebRequest();

            if (getGeneratorsRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Get generators Error: " + getGeneratorsRequest.error);
                getGeneratorsRequest.Dispose();
            }
            else
            {
                var generators =
                    JsonConvert.DeserializeObject<List<Generator>>(getGeneratorsRequest.downloadHandler.text);
                
                getGeneratorsRequest.Dispose();

                return generators;
            }

            return null;
        }

        public static async Task<int> CreateImagine(List<GeneratorField> generatorFields, string generator, string apiKey)
        {
            // Create a dictionary of string keys and values to hold the JSON POST params
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("generator", generator);

            foreach (var field in generatorFields)
            {
                if (field.value != "")
                {
                    parameters.Add(field.key, field.value);
                }
            }

            string parametersJsonString = JsonConvert.SerializeObject(parameters);

            var createImagineRequest = new UnityWebRequest();
            createImagineRequest.url = "https://backend.blockadelabs.com/api/v1/imagine/requests?api_key=" + apiKey;
            createImagineRequest.method = "POST";
            createImagineRequest.downloadHandler = new DownloadHandlerBuffer();
            createImagineRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(parametersJsonString));
            createImagineRequest.timeout = 60;
            createImagineRequest.SetRequestHeader("Accept", "application/json");
            createImagineRequest.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

            await createImagineRequest.SendWebRequest();

            if (createImagineRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Create Imagine Error: " + createImagineRequest.error);
                createImagineRequest.Dispose();
            }
            else
            {
                var result = JsonConvert.DeserializeObject<CreateImagineResult>(createImagineRequest.downloadHandler.text);
                
                createImagineRequest.Dispose();

                if (result?.request == null)
                {
                    return 0;
                }

                return int.Parse(result.request.id);
            }
            
            return 0;
        }

        public static async Task<Dictionary<string, string>> GetImagine(int imagineId, string apiKey)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
           
            var getImagineRequest = UnityWebRequest.Get(
                "https://backend.blockadelabs.com/api/v1/imagine/requests/" + imagineId + "?api_key=" + apiKey
            );

            await getImagineRequest.SendWebRequest();

            if (getImagineRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Get Imagine Error: " + getImagineRequest.error);
                getImagineRequest.Dispose();
            }
            else
            {
                var status = JsonConvert.DeserializeObject<GetImagineResult>(getImagineRequest.downloadHandler.text);
                
                getImagineRequest.Dispose();

                if (status?.request != null)
                {
                    if (status.request?.status == "complete")
                    {
                        result.Add("textureUrl", status.request.file_url);
                        result.Add("prompt", status.request.prompt);
                    }
                }
            }

            return result;
        }
        
        public static async Task<byte[]> GetImagineImage(string textureUrl)
        {
            var imagineImageRequest = UnityWebRequest.Get(textureUrl);
            await imagineImageRequest.SendWebRequest();

            if (imagineImageRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Get Imagine Image Error: " + imagineImageRequest.error);
                imagineImageRequest.Dispose();
            }
            else
            {
                var image = imagineImageRequest.downloadHandler.data;
                imagineImageRequest.Dispose();

                return image;
            }

            return null;
        }
    }
}