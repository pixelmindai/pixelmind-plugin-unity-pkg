using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace PixelmindSDK
{
    public class ApiRequests
    {
        public static async Task<List<SkyboxStyle>> GetSkyboxStyles(string apiKey)
        {
            var getSkyboxStylesRequest = UnityWebRequest.Get(
                // "https://backend.blockadelabs.com/api/v1/skybox" + "?api_key=" + apiKey
                "https://backend.blockadelabs.com/api/v1/skybox" + "?api_key=" + apiKey
            );
            
            Debug.Log(getSkyboxStylesRequest);

            await getSkyboxStylesRequest.SendWebRequest();
            
            Debug.Log(getSkyboxStylesRequest.result);

            if (getSkyboxStylesRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Get skybox styles error: " + getSkyboxStylesRequest.error);
                getSkyboxStylesRequest.Dispose();
            }
            else
            {
                Debug.Log("skyboxStyles before");
                Debug.Log(getSkyboxStylesRequest.downloadHandler.text);
                var skyboxStyles = JObject.Parse(getSkyboxStylesRequest.downloadHandler.text);
                //     JsonConvert.DeserializeObject<SkyboxStyleResult>(getSkyboxStylesRequest.downloadHandler.text);

                // var test = JsonConvert.DeserializeObject<JObject>(getSkyboxStylesRequest.downloadHandler.text);
                // var jo = JObject.Parse(getSkyboxStylesRequest.downloadHandler.text);
                
                var skyboxStylesList = new List<SkyboxStyle>();

                foreach (var item in skyboxStyles)
                {
                    if (int.TryParse(item.Key, out int n))
                    {
                        // skyboxStylesList.Add(item.Value["name"].ToString());
                        Debug.Log(item.Value["user_prompts"]["inputs"]);
                        Debug.Log(item.Value["user_prompts"]["inputs"].ToList());
                        Debug.Log(item.Value["user_prompts"]["inputs"].ToString());
                        // Debug.Log(item.Value["user_prompts"]["inputs"].ToObject<UserInput>().ToString());

                        var skyboxStyle = new SkyboxStyle(
                            int.Parse(item.Value["id"].ToString()),
                            item.Value["name"].ToString()
                            // item.Value["user_prompts"]["inputs"].ToString()
                            // item.Value["user_prompts"]["inputs"].ToObject<UserInput>()
                            // item.Value["user_prompts"]["inputs"].ToString()
                        );
                        
                        Debug.Log(skyboxStyle);

                        var userInputs = item.Value["user_prompts"]["inputs"].Children().OfType<JProperty>();

                        foreach (var userInput in userInputs)
                        {
                            Debug.Log(userInput);
                            Debug.Log(userInput.Name);
                            Debug.Log(userInput.Value["id"]);
                            Debug.Log(userInput.Value["name"]);
                            Debug.Log(userInput.Value["placeholder"]);
                            // Debug.Log(inputProperty.Value<int?>("id"));
                            // Debug.Log(input.Key);
                            skyboxStyle.userInputs.Add(
                                new UserInput(
                                    userInput.Name,
                                    int.Parse(userInput.Value["id"].ToString()),
                                    userInput.Value["name"].ToString(),
                                    userInput.Value["placeholder"].ToString()
                                )
                            );
                            
                            Debug.Log(skyboxStyle.userInputs);
                        }

                        skyboxStylesList.Add(skyboxStyle);
                    }
                }
                
                Debug.Log(skyboxStylesList.Count);
                Debug.Log(skyboxStylesList.ToString());
                Debug.Log(skyboxStylesList[0]);
                Debug.Log(skyboxStylesList[0].id);
                Debug.Log(skyboxStylesList[0].name);
                Debug.Log(skyboxStylesList[0].userInputs);
                Debug.Log(skyboxStylesList[0].userInputs[0]);
                Debug.Log(skyboxStylesList[0].userInputs[0].id);
                Debug.Log(skyboxStylesList[0].userInputs[0].key);

                getSkyboxStylesRequest.Dispose();

                return skyboxStylesList;
            }

            return null;
        }
        
        public static async Task<List<Generator>> GetGenerators(string apiKey)
        {
            var getGeneratorsRequest = UnityWebRequest.Get(
                "https://backend.blockadelabs.com/api/v1/generators" + "?api_key=" + apiKey
            );

            await getGeneratorsRequest.SendWebRequest();

            if (getGeneratorsRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Get generators error: " + getGeneratorsRequest.error);
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
        
        public static async Task<int> CreateSkybox(List<SkyboxStyleField> skyboxStyleFields, int id, string apiKey)
        {
            // Create a dictionary of string keys and values to hold the JSON POST params
            Dictionary<string, Dictionary<string, string>> parameters = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> userInputs = new Dictionary<string, string>();
            parameters.Add("prompt", new Dictionary<string, string>());

            foreach (var field in skyboxStyleFields)
            {
                if (field.value != "")
                {
                    Debug.Log(field.key);
                    Debug.Log(field.value);
                    userInputs.Add(field.key.Trim('[', ']'), field.value);
                }
            }

            parameters["prompt"] = userInputs;
            
            Debug.Log(string.Join(", ", parameters["prompt"]));
            
            Debug.Log(parameters.ToString());
            Debug.Log(parameters["prompt"].ToString());
            Debug.Log(userInputs.ToString());
            Debug.Log(id);
            Debug.Log("https://backend.blockadelabs.com/api/v1/skybox/submit/" + id + "?api_key=" + apiKey);

            string parametersJsonString = JsonConvert.SerializeObject(parameters);

            var createSkyboxRequest = new UnityWebRequest();
            createSkyboxRequest.url = "https://backend.blockadelabs.com/api/v1/skybox/submit/" + id + "?api_key=" + apiKey;
            createSkyboxRequest.method = "POST";
            createSkyboxRequest.downloadHandler = new DownloadHandlerBuffer();
            createSkyboxRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(parametersJsonString));
            createSkyboxRequest.timeout = 60;
            createSkyboxRequest.SetRequestHeader("Accept", "application/json");
            createSkyboxRequest.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

            await createSkyboxRequest.SendWebRequest();
            
            Debug.Log(createSkyboxRequest.result);
            Debug.Log(createSkyboxRequest.downloadHandler.text);

            // return 0;

            if (createSkyboxRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Create Skybox Error: " + createSkyboxRequest.error);
                createSkyboxRequest.Dispose();
            }
            else
            {
                var result = JsonConvert.DeserializeObject<CreateSkyboxResult>(createSkyboxRequest.downloadHandler.text);
                
                Debug.Log(result);
                Debug.Log(result.imaginations);
                Debug.Log(result.imaginations[0].id);
                createSkyboxRequest.Dispose();
            
                if (result?.imaginations[0] == null)
                {
                    return 0;
                }
            
                return int.Parse(result.imaginations[0].id);
            }
            
            return 0;
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