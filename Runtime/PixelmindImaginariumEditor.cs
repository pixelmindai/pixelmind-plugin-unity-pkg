using UnityEditor;
using UnityEngine;

namespace PixelmindSDK
{
#if UNITY_EDITOR
    [CustomEditor(typeof(PixelmindImaginarium))]
    public class PixelmindImaginariumEditor : Editor
    {
        private SerializedProperty assignToMaterial;
        private SerializedProperty assignToSpriteRenderer;
        private SerializedProperty enableGUI;
        private SerializedProperty enableSkyboxGUI;
        private SerializedProperty resultImage;
        private SerializedProperty apiKey;
        private SerializedProperty generatorFields;
        private SerializedProperty skyboxStyleFields;
        private SerializedProperty generators;
        private SerializedProperty skyboxStyles;
        private SerializedProperty generatorOptions;
        private SerializedProperty skyboxStyleOptions;
        private SerializedProperty generatorOptionsIndex;
        private SerializedProperty skyboxStyleOptionsIndex;
        private bool showApi = true;
        private bool showBasic = true;
        private bool showGenerators = true;
        private bool showSkybox = true;
        private bool showOutput = true;

        void OnEnable()
        {
            assignToMaterial = serializedObject.FindProperty("assignToMaterial");
            assignToSpriteRenderer = serializedObject.FindProperty("assignToSpriteRenderer");
            enableGUI = serializedObject.FindProperty("enableGUI");
            enableSkyboxGUI = serializedObject.FindProperty("enableSkyboxGUI");
            apiKey = serializedObject.FindProperty("apiKey");
            resultImage = serializedObject.FindProperty("resultImage");
            generatorFields = serializedObject.FindProperty("generatorFields");
            skyboxStyleFields = serializedObject.FindProperty("skyboxStyleFields");
            generators = serializedObject.FindProperty("generators");
            skyboxStyles = serializedObject.FindProperty("skyboxStyles");
            generatorOptions = serializedObject.FindProperty("generatorOptions");
            skyboxStyleOptions = serializedObject.FindProperty("skyboxStyleOptions");
            generatorOptionsIndex = serializedObject.FindProperty("generatorOptionsIndex");
            skyboxStyleOptionsIndex = serializedObject.FindProperty("skyboxStyleOptionsIndex");
        }

        public override void OnInspectorGUI()
        {
            EditorUtility.SetDirty(target);

            serializedObject.Update();

            var pixelmindImaginarium = (PixelmindImaginarium)target;

            showApi = EditorGUILayout.Foldout(showApi, "Api");

            if (showApi)
            {
                EditorGUILayout.PropertyField(apiKey);
            }

            showBasic = EditorGUILayout.Foldout(showBasic, "Basic Settings");

            if (showBasic)
            {
                EditorGUILayout.PropertyField(enableGUI);
                EditorGUILayout.PropertyField(enableSkyboxGUI);
                EditorGUILayout.PropertyField(assignToSpriteRenderer);
                EditorGUILayout.PropertyField(assignToMaterial);
            }

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                showGenerators = EditorGUILayout.Foldout(showGenerators, "Generators");

                if (showGenerators)
                {
                    if (GUILayout.Button("Get Generators"))
                    {
                        _ = pixelmindImaginarium.GetGeneratorsWithFields();
                    }

                    // Iterate over generator fields and render them in the GUI
                    if (pixelmindImaginarium.generatorFields.Count > 0)
                    {
                        RenderEditorFields(pixelmindImaginarium);
                    }
                    
                    if (pixelmindImaginarium.PercentageCompleted() >= 0 && pixelmindImaginarium.PercentageCompleted() < 100)
                    {
                        if (GUILayout.Button("Cancel (" + pixelmindImaginarium.PercentageCompleted() + "%)"))
                        {
                            pixelmindImaginarium.Cancel();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Generate"))
                        {
                            _ = pixelmindImaginarium.InitializeGeneration(
                                pixelmindImaginarium.generatorFields,
                                pixelmindImaginarium.generators[pixelmindImaginarium.generatorOptionsIndex].generator
                            );
                        }
                    }
                }
                
                showSkybox = EditorGUILayout.Foldout(showSkybox, "Skybox");

                if (showSkybox)
                {
                    if (GUILayout.Button("Get Styles"))
                    {
                        _ = pixelmindImaginarium.GetSkyboxStyleOptions();
                    }

                    // Iterate over skybox style fields and render them in the GUI
                    if (pixelmindImaginarium.skyboxStyleFields.Count > 0)
                    {
                        RenderSkyboxEditorFields(pixelmindImaginarium);
                    }
                    
                    if (pixelmindImaginarium.PercentageCompleted() >= 0 && pixelmindImaginarium.PercentageCompleted() < 100)
                    {
                        if (GUILayout.Button("Cancel (" + pixelmindImaginarium.PercentageCompleted() + "%)"))
                        {
                            pixelmindImaginarium.Cancel();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Generate Skybox"))
                        {
                            _ = pixelmindImaginarium.InitializeSkyboxGeneration(
                                pixelmindImaginarium.skyboxStyleFields,
                                pixelmindImaginarium.skyboxStyles[pixelmindImaginarium.skyboxStyleOptionsIndex].id
                            );
                        }
                    }
                }

                showOutput = EditorGUILayout.Foldout(showOutput, "Output");

                if (showOutput)
                {
                    EditorGUILayout.PropertyField(resultImage);
                    if (pixelmindImaginarium.previewImage != null) GUILayout.Box(pixelmindImaginarium.previewImage);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderEditorFields(PixelmindImaginarium pixelmindImaginarium)
        {
            EditorGUI.BeginChangeCheck();

            pixelmindImaginarium.generatorOptionsIndex = EditorGUILayout.Popup(
                pixelmindImaginarium.generatorOptionsIndex,
                pixelmindImaginarium.generatorOptions
            );

            if (EditorGUI.EndChangeCheck())
            {
                pixelmindImaginarium.GetGeneratorFields(pixelmindImaginarium.generatorOptionsIndex);
            }

            foreach (var field in pixelmindImaginarium.generatorFields)
            {
                // Begin horizontal layout
                EditorGUILayout.BeginHorizontal();
                
                var required = field.required ? "*" : "";
                // Create label for field
                EditorGUILayout.LabelField(field.key + required);

                // Create text field for field value
                field.value = EditorGUILayout.TextField(field.value);

                // End horizontal layout
                EditorGUILayout.EndHorizontal();
            }
        }
        
        private void RenderSkyboxEditorFields(PixelmindImaginarium pixelmindImaginarium)
        {
            EditorGUI.BeginChangeCheck();

            pixelmindImaginarium.skyboxStyleOptionsIndex = EditorGUILayout.Popup(
                pixelmindImaginarium.skyboxStyleOptionsIndex,
                pixelmindImaginarium.skyboxStyleOptions
            );

            if (EditorGUI.EndChangeCheck())
            {
                pixelmindImaginarium.GetSkyboxStyleFields(pixelmindImaginarium.skyboxStyleOptionsIndex);
            }

            foreach (var field in pixelmindImaginarium.skyboxStyleFields)
            {
                // Begin horizontal layout
                EditorGUILayout.BeginHorizontal();
                
                // Create label for field
                EditorGUILayout.LabelField(field.name + "*");
            
                // Create text field for field value
                field.value = EditorGUILayout.TextField(field.value);
            
                // End horizontal layout
                EditorGUILayout.EndHorizontal();
            }
        }
    }
#endif
}