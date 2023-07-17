using MMD_URP;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace MMD_URP
{

    public class GameSystem
    {
        //[System.NonSerialized]
        public static ImportData importData = new ImportData();
        public static string pmxPath;
        public static string vmdPath;
        public static Convertor convertor;
    }

    //public class GameSystem : MonoBehaviour
    //{

    //    private Dictionary<string, System.Action> UIButtonFunctions = new Dictionary<string, System.Action>();
    //    private Dictionary<string, EventCallback<ChangeEvent<bool>>> UIToggleFunctions = new Dictionary<string, EventCallback<ChangeEvent<bool>>>();
    //    private Dictionary<string, EventCallback<ChangeEvent<float>>> UISliderFunctions = new Dictionary<string, EventCallback<ChangeEvent<float>>>();
    //    private Dictionary<string, EventCallback<ChangeEvent<int>>> UISliderIntFunctions = new Dictionary<string, EventCallback<ChangeEvent<int>>>();
    //    private Dictionary<string, EventCallback<ChangeEvent<Enum>>> UIEnumFieldFunctions = new Dictionary<string, EventCallback<ChangeEvent<Enum>>>();
    //    private Dictionary<string, EventCallback<ChangeEvent<int>>> UIIntegerFieldFunctions = new Dictionary<string, EventCallback<ChangeEvent<int>>>();
    //    private Dictionary<string, EventCallback<ChangeEvent<float>>> UIFloatFieldFunctions = new Dictionary<string, EventCallback<ChangeEvent<float>>>();
    //    private HashSet<string> foldoutExpanded = new HashSet<string> { "AssetImportFoldout", "ControlFoldout", "VideoFoldout", "SkyboxFoldout", "WaterFoldout", "MaterialStandardFoldout" };
    //    private MonoBehaviour coroutineExecuter;

    //    //Subsystem
    //    private WeatherSystem weatherSystem;
    //    private Material waterMaterial;

    //    //temp data to avoid GC
    //    private Vector3 tempVector = Vector3.zero;

    //    public static CameraController cameraManager;
    //    public static ScreenRecorder recorder;
    //    public static RecordVideoRenderPass recordPass;
    //    public static LightManager lightManager;

    //    [System.NonSerialized]
    //    public static ImportData importData;
    //    public static Material materialSelected;
    //    public static string TexPath;
    //    public static TextListView boneList;
    //    private static VisualElement CelShadingContainer;
    //    private static VisualElement RampShadingContainer;
    //    private static VisualElement CellBandsShadingContainer;
    //    private static ScrollView PBR_GGXContainer;
    //    private static ScrollView StylizedContainer;
    //    private static ScrollView BlinnPhongContainer;
    //    private static ScrollView FresnelRimContainer;
    //    private static ScrollView ScreenSpaceRimContainer;
    //    private static ScrollView AnisotropyContainer;
    //    private static ScrollView AngleRingContainer;
    //    private static ScrollView PBR_GGXHairContainer;
    //    private static ScrollView StylizedHairContainer;
    //    private static ScrollView FresnelHairRimContainer;
    //    private static VisualElement CelFaceShadingContainer;
    //    private static VisualElement CellBandsFaceShadingContainer;
    //    private static VisualElement SDFFaceShadingContainer;
    //    private static VisualElement RampFaceShadingContainer;
    //    public static VisualElement PopOutConatiner;
    //    public static Label KeyframeFunction;
    //    public static IntegerField FrameIndex;
    //    public static FloatField FrameValue;
    //    public static Button Done;
    //    private static int keyframeIndex;
    //    private static float keyframeValue;
    //    private static float NormapScale;
    //    public static Foldout MaterialNameFoldout;
    //    private static Foldout UseOutlineFoldout;
    //    private static Foldout UseNormalMapFoldout;
    //    //material property
    //    private static ColorRGBA metallicChannel;
    //    private static float metallic;
    //    private static ColorRGBA smoothnessChannel;
    //    private static float smoothness;
    //    private static ColorRGBA occlusionChannel;
    //    private static float occlusion;
    //    private static DiffuseShadingMode diffuseShadingMode;
    //    private static DiffuseShadingHairMode diffuseShadingHairMode;
    //    private static DiffuseShadingFaceMode diffuseShadingFaceMode;
    //    private static bool useHarfLambert;
    //    private static bool useRadianceOcclusion;
    //    private static bool useSpecularMask;
    //    private static int cellBands;
    //    private static float cellThreshold;
    //    private static float cellSmoothing;
    //    private static float rampU;
    //    private static float rampV;
    //    private static float SpecularAAVariant;
    //    private static float SpecularAAThreshold;
    //    private static float StylizedSpecularSize;
    //    private static float StylizedSpecularSoftness;
    //    private static float SpecularColorAlbedoWeight;
    //    private static float BlinnPhongShininess;
    //    private static float OutlineWidth; 
    //    private static float AnisoShiftScale;
    //    private static float SpecularWidth;
    //    private static float AngleRingIntensity;
    //    private static float ShadowThreshold;
    //    private static float ShadowSoftness;
    //    private static float WaterDistance;
    //    private static float WaterRefractionSpeed;
    //    private static float WaterFoamScale;
    //    private static float WaterFoamCutoff;
    //    private static float WaterSmoothness;
    //    private static SpecularShadingMode specularShadingMode;
    //    private static SpecularHairShadingMode specularHairShadingMode;
    //    private static RimMode rimMode;
    //    private static bool useOutline;
    //    private static bool useNormalMap;
    //    private static FernShaders fernShader = FernShaders.Standard;
    //    private static KeyframeActionState KeyframeAction;
    //    private static Shader standardShader;
    //    private static Shader hairShader;
    //    private static Shader faceShader;

    //    // Start is called before the first frame update
    //    void Start()
    //    {
    //        coroutineExecuter = this;
    //        initUIFunctions();
    //        Init();
    //    }

    //    private void Update()
    //    {
    //        weatherSystem.Update();
    //    }

    //    public void Init()
    //    {
    //        importData = new ImportData();
    //        VisualTreeAsset mainInterface = Resources.Load<VisualTreeAsset>("UI/MainScreenTemplate");
    //        var UIDocument = GetComponent<UIDocument>();
    //        UIDocument.visualTreeAsset = mainInterface;
    //        settingUpUI(UIDocument.rootVisualElement);

    //        boneList = UIDocument.rootVisualElement.Q<TextListView>("BoneListView");

    //        CelShadingContainer = UIDocument.rootVisualElement.Q<VisualElement>("CelShadingContainer");
    //        RampShadingContainer = UIDocument.rootVisualElement.Q<VisualElement>("RampShadingContainer");
    //        CellBandsShadingContainer = UIDocument.rootVisualElement.Q<VisualElement>("CellBandsShadingContainer");

    //        PBR_GGXContainer = UIDocument.rootVisualElement.Q<ScrollView>("PBR_GGXContainer");
    //        StylizedContainer = UIDocument.rootVisualElement.Q<ScrollView>("StylizedContainer");
    //        BlinnPhongContainer = UIDocument.rootVisualElement.Q<ScrollView>("BlinnPhongContainer");
    //        FresnelRimContainer = UIDocument.rootVisualElement.Q<ScrollView>("FresnelRimContainer");
    //        ScreenSpaceRimContainer = UIDocument.rootVisualElement.Q<ScrollView>("ScreenSpaceRimContainer");
    //        AnisotropyContainer = UIDocument.rootVisualElement.Q<ScrollView>("AnisotropyContainer");
    //        AngleRingContainer = UIDocument.rootVisualElement.Q<ScrollView>("AngleRingContainer");
    //        PBR_GGXHairContainer = UIDocument.rootVisualElement.Q<ScrollView>("PBR_GGXHairContainer");
    //        StylizedHairContainer = UIDocument.rootVisualElement.Q<ScrollView>("StylizedHairContainer");
    //        FresnelHairRimContainer = UIDocument.rootVisualElement.Q<ScrollView>("FresnelHairRimContainer");

    //        CelFaceShadingContainer = UIDocument.rootVisualElement.Q<VisualElement>("CelFaceShadingContainer");
    //        CellBandsFaceShadingContainer = UIDocument.rootVisualElement.Q<VisualElement>("CellBandsFaceShadingContainer");
    //        SDFFaceShadingContainer = UIDocument.rootVisualElement.Q<VisualElement>("SDFFaceShadingContainer");
    //        RampFaceShadingContainer = UIDocument.rootVisualElement.Q<VisualElement>("RampFaceShadingContainer");

    //        PopOutConatiner = UIDocument.rootVisualElement.Q<VisualElement>("PopOutConatiner");
    //        KeyframeFunction = UIDocument.rootVisualElement.Q<Label>("KeyframeFunction");
    //        FrameIndex = UIDocument.rootVisualElement.Q<IntegerField>("FrameIndex");
    //        FrameValue = UIDocument.rootVisualElement.Q<FloatField>("FrameValue");
    //        Done = UIDocument.rootVisualElement.Q<Button>("DoneButton");

    //        MaterialNameFoldout = UIDocument.rootVisualElement.Q<Foldout>("MaterialNameFoldout");

    //        UseOutlineFoldout = UIDocument.rootVisualElement.Q<Foldout>("UseOutlineFoldout");
    //        UseNormalMapFoldout = UIDocument.rootVisualElement.Q<Foldout>("UseNormalMapFoldout");

    //        standardShader = Shader.Find("FernRender/URP/FERNNPRStandard");
    //        faceShader = Shader.Find("FernRender/URP/FERNNPRFace");
    //        hairShader = Shader.Find("FernRender/URP/FERNNPRHair");

    //        weatherSystem = new WeatherSystem();

    //        waterMaterial = Resources.Load<Material>("Shaders/Water");

    //    }

    //    private void settingUpUI(VisualElement root)
    //    {
    //        root.Query<TextField>().ForEach((textField) => {
    //            textField.style.width = Length.Percent(40);
    //        });

    //        root.Query<Foldout>().ForEach((foldout) => {
    //            foldout.RegisterValueChangedCallback((ChangeEvent<bool> evt) =>
    //            {
    //                if (foldoutExpanded.Contains(foldout.name))
    //                {
    //                    if (evt.newValue)
    //                        foldout.style.height = Length.Percent(80);
    //                    else
    //                        foldout.style.height = Length.Percent(20);
    //                }
    //            });
    //            foldout.Q<VisualElement>("unity-content").style.height = Length.Percent(100);
    //        });

    //        root.Query<Button>().ForEach((button) => {
    //            if (UIButtonFunctions.TryGetValue(button.name, out var funcions))
    //                button.clickable.clicked += funcions;
    //        });

    //        root.Query<Toggle>().ForEach((toggle) =>
    //        {
    //            if (UIToggleFunctions.TryGetValue(toggle.name, out var funcions))
    //                toggle.RegisterValueChangedCallback(funcions);
    //        });

    //        root.Query<Slider>().ForEach((slider) =>
    //        {
    //            if (UISliderFunctions.TryGetValue(slider.name, out var funcions))
    //                slider.RegisterValueChangedCallback(funcions);
    //        });

    //        root.Query<EnumField>().ForEach((field) =>
    //        {
    //            if (UIEnumFieldFunctions.TryGetValue(field.name, out var funcions))
    //                field.RegisterValueChangedCallback<System.Enum>(funcions);
    //        });

    //        root.Query<IntegerField>().ForEach((field) =>
    //        {
    //            if (UIIntegerFieldFunctions.TryGetValue(field.name, out var funcions))
    //                field.RegisterValueChangedCallback<int>(funcions);
    //        });

    //        root.Query<FloatField>().ForEach((field) =>
    //        {
    //            if (UIFloatFieldFunctions.TryGetValue(field.name, out var funcions))
    //                field.RegisterValueChangedCallback<float>(funcions);
    //        });
    //    }

    //    static public void settingUpBoneList()
    //    {
    //        if (boneList.gameObjectSelected != CameraController.gameObjectSelected && importData.IsModelExist(CameraController.gameObjectSelected, out BoneManager boneManager)) {
    //            foreach (var name in boneManager.boneTransforms.Keys)
    //            {
    //                boneList.AddItem(name);
    //                boneList.gameObjectSelected = CameraController.gameObjectSelected;
    //            }
    //        }
    //    }

    //    //Add UI elements functions here.
    //    private void initUIFunctions()
    //    {
    //        //Button
    //        UIButtonFunctions.Add("ImportModelButton", () => { ButtonPMX.OpenBrowser(coroutineExecuter); });
    //        UIButtonFunctions.Add("ImportMotionButton", () => { ButtonVMD.OpenBrowser(coroutineExecuter); });
    //        //UIButtonFunctions.Add("ImportTextureButton", () => { ButtonTexFolder.OpenBrowser(coroutineExecuter); });
    //        UIButtonFunctions.Add("ProcessButton", () => { Process.CleanModel(); });
    //        UIButtonFunctions.Add("PlayButton", () => { importData.Play(); });
    //        UIButtonFunctions.Add("PauseButton", () => { importData.Pause(); });
    //        UIButtonFunctions.Add("RestartButton", () => { importData.Restart(); });
    //        UIButtonFunctions.Add("BakeButton", () => { importData.Bake(); });
    //        UIButtonFunctions.Add("BaseMapButton", () => { SetTexture("_BaseMap"); });
    //        UIButtonFunctions.Add("NormapMapButton", () => { SetTexture("_NORMALMAP"); });
    //        UIButtonFunctions.Add("LightMapButton", () => { SetTexture("_LightMap"); });
    //        UIButtonFunctions.Add("RampMapButton", () => { SetTexture("_DiffuseRampMap"); });
    //        UIButtonFunctions.Add("AnisoShiftMapButton", () => { SetTexture("_AnisoShiftMap"); });
    //        UIButtonFunctions.Add("DoneButton", () => 
    //        {
    //            for (int i = 0; i < 6; i++)
    //                if (boneList._curveView.isCurvesVisible[i])
    //                    importData.models[0].animator.modifyKeyframe(i, keyframeIndex, keyframeValue, KeyframeAction);
    //            PopOutConatiner.style.display = DisplayStyle.None;
    //        });
    //        UIButtonFunctions.Add("StartRecButton", () =>
    //        {
    //            recorder.StartRecord();
    //        });
    //        UIButtonFunctions.Add("StopRecButton", () =>
    //        {
    //            recorder.StopRecord();
    //        });
    //        UIButtonFunctions.Add("CancelRecButton", () =>
    //        {

    //        });
    //        //UIButtonFunctions.Add("BaseMapButton", () => { SetTexture("_BaseMap"); });
    //        //UIButtonFunctions.Add("ClearMotionButton", () => { importData.DestroyAnimations(); });
    //        //UIButtonFunctions.Add("ClearAllButton", () => { importData.DestroyModels(); });

    //        //Toggle
    //        UIToggleFunctions.Add("Lightening", (ChangeEvent<bool> evt) =>{ weatherSystem.lightening = evt.newValue;});
    //        UIToggleFunctions.Add("HarfLambert", (ChangeEvent<bool> evt) => 
    //        { 
    //            useHarfLambert = evt.newValue; 
    //            if(useHarfLambert) materialSelected.EnableKeyword("_UseHalfLambert"); 
    //            else materialSelected.DisableKeyword("_UseHalfLambert");
    //        });
    //        UIToggleFunctions.Add("RadianceOcclusion", (ChangeEvent<bool> evt) => 
    //        { 
    //            useRadianceOcclusion = evt.newValue;
    //            if (useRadianceOcclusion) materialSelected.EnableKeyword("_UseRadianceOcclusion");
    //            else materialSelected.DisableKeyword("_UseRadianceOcclusion");
    //        });
    //        UIToggleFunctions.Add("UseNormalMap", (ChangeEvent<bool> evt) =>
    //        {
    //            useNormalMap = evt.newValue;
    //            if (useNormalMap)
    //                UseNormalMapFoldout.style.display = DisplayStyle.Flex;
    //            else
    //                UseNormalMapFoldout.style.display = DisplayStyle.None;

    //            if (useNormalMap) materialSelected.EnableKeyword("_NORMALMAP");
    //            else materialSelected.DisableKeyword("_NORMALMAP");
    //        });
    //        UIToggleFunctions.Add("UseOutline", (ChangeEvent<bool> evt) =>
    //        {
    //            useOutline = evt.newValue;
    //            if(useOutline)
    //                UseOutlineFoldout.style.display = DisplayStyle.Flex;
    //            else
    //                UseOutlineFoldout.style.display = DisplayStyle.None;

    //            if (useOutline) materialSelected.EnableKeyword("_Outline");
    //            else materialSelected.DisableKeyword("_Outline");
    //        });
    //        UIToggleFunctions.Add("SpecularMaskToggle", (ChangeEvent<bool> evt) =>
    //        {
    //            useSpecularMask = evt.newValue;
    //            if (useSpecularMask) materialSelected.EnableKeyword("_SPECULARMASK");
    //            else materialSelected.DisableKeyword("_SPECULARMASK");
    //        });

    //        //Slider
    //        UISliderFunctions.Add("TimeSlider", (ChangeEvent<float> evt) =>
    //        {
    //            tempVector = lightManager.gameObject.transform.eulerAngles;
    //            tempVector.x = (evt.newValue + 6) % 24 * 15;
    //            lightManager.gameObject.transform.eulerAngles = tempVector;
    //        });
    //        UISliderFunctions.Add("MetallicSlider", (ChangeEvent<float> evt) => { metallic = evt.newValue; materialSelected.SetFloat("_Metallic", metallic);});
    //        UISliderFunctions.Add("SmoothSlider", (ChangeEvent<float> evt) => { smoothness = evt.newValue; materialSelected.SetFloat("_Smoothness", smoothness); });
    //        UISliderFunctions.Add("OcclusionSlider", (ChangeEvent<float> evt) => { occlusion = evt.newValue; materialSelected.SetFloat("_OcclusionStrength", occlusion); });
    //        UISliderFunctions.Add("CellSmoothingSlider", (ChangeEvent<float> evt) => { cellSmoothing = evt.newValue; materialSelected.SetFloat("_CELLSmoothing", cellSmoothing); });
    //        UISliderFunctions.Add("CellThresholdSlider", (ChangeEvent<float> evt) => { cellThreshold = evt.newValue; materialSelected.SetFloat("_CELLThreshold", cellThreshold); });
    //        UISliderFunctions.Add("RampUOffsetSlider", (ChangeEvent<float> evt) => { rampU = evt.newValue; materialSelected.SetFloat("_RampMapUOffset", rampU); });
    //        UISliderFunctions.Add("RampVOffsetSlider", (ChangeEvent<float> evt) => { rampV = evt.newValue; materialSelected.SetFloat("_RampMapVOffset", rampV); });
    //        UISliderFunctions.Add("SpecularAAVariantSlider", (ChangeEvent<float> evt) => { SpecularAAVariant = evt.newValue; materialSelected.SetFloat("_SpaceScreenVariant", SpecularAAVariant); });
    //        UISliderFunctions.Add("SpecularAAThresholdSlider", (ChangeEvent<float> evt) => { SpecularAAThreshold = evt.newValue; materialSelected.SetFloat("_SpecularAAThreshold", SpecularAAThreshold); });
    //        UISliderFunctions.Add("StylizedSpecularSizeSlider", (ChangeEvent<float> evt) => { StylizedSpecularSize = evt.newValue; materialSelected.SetFloat("_StylizedSpecularSize", StylizedSpecularSize); });
    //        UISliderFunctions.Add("StylizedSpecularSoftnessSlider", (ChangeEvent<float> evt) => { StylizedSpecularSoftness = evt.newValue; materialSelected.SetFloat("_StylizedSpecularSoftness", StylizedSpecularSoftness); });
    //        UISliderFunctions.Add("SpecularColorAlbedoWeightSlider", (ChangeEvent<float> evt) => { SpecularColorAlbedoWeight = evt.newValue; materialSelected.SetFloat("_StylizedSpecularAlbedoWeight", SpecularColorAlbedoWeight); });
    //        UISliderFunctions.Add("BlinnPhongShininessSlider", (ChangeEvent<float> evt) => { BlinnPhongShininess = evt.newValue; materialSelected.SetFloat("_Shininess", BlinnPhongShininess); });
    //        UISliderFunctions.Add("OutlineWidthSlider", (ChangeEvent<float> evt) => { OutlineWidth = evt.newValue; materialSelected.SetFloat("_OutlineWidth", OutlineWidth); });
    //        UISliderFunctions.Add("AnisoShiftScaleSlider", (ChangeEvent<float> evt) => { AnisoShiftScale = evt.newValue; materialSelected.SetFloat("_AnisoShiftScale", AnisoShiftScale); });
    //        UISliderFunctions.Add("SpecularWidthSlider", (ChangeEvent<float> evt) => { SpecularWidth = evt.newValue; materialSelected.SetFloat("_AngleRingWidth", SpecularWidth); });
    //        UISliderFunctions.Add("AngleRingIntensitySlider", (ChangeEvent<float> evt) => { AngleRingIntensity = evt.newValue; materialSelected.SetFloat("_AngleRingIntensity", AngleRingIntensity); });
    //        UISliderFunctions.Add("ShadowThresholdSlider", (ChangeEvent<float> evt) => { ShadowThreshold = evt.newValue; materialSelected.SetFloat("_AngleRingThreshold", ShadowThreshold); });
    //        UISliderFunctions.Add("ShadowSoftnessSlider", (ChangeEvent<float> evt) => { ShadowSoftness = evt.newValue; materialSelected.SetFloat("_AngleRingSoftness", ShadowSoftness); });
    //        UISliderFunctions.Add("DistanceSlider", (ChangeEvent<float> evt) => { WaterDistance = evt.newValue; waterMaterial.SetFloat("_Distance", WaterDistance); });
    //        UISliderFunctions.Add("RefractionSpeedSlider", (ChangeEvent<float> evt) => { WaterRefractionSpeed = evt.newValue; waterMaterial.SetFloat("_RefractionSpeed", WaterRefractionSpeed); });
    //        UISliderFunctions.Add("FoamScaleSlider", (ChangeEvent<float> evt) => { WaterFoamScale = evt.newValue; waterMaterial.SetFloat("_FoamScale", WaterFoamScale); });
    //        UISliderFunctions.Add("FoamCutoffSlider", (ChangeEvent<float> evt) => { WaterFoamCutoff = evt.newValue; waterMaterial.SetFloat("_FoamSpeed", WaterFoamCutoff); });
    //        UISliderFunctions.Add("SmoothnessSlider", (ChangeEvent<float> evt) => { WaterSmoothness = evt.newValue; waterMaterial.SetFloat("_Smoothness", WaterSmoothness); });


    //        //SliderInt
    //        UISliderIntFunctions.Add("CellBandsSlider", (ChangeEvent<int> evt) => { cellBands = evt.newValue; materialSelected.SetInt("_CellBands", cellBands); });


    //        //EnumField
    //        UIEnumFieldFunctions.Add("Daytime", (ChangeEvent<Enum> evt) =>{ weatherSystem.daytime = (DayTime) evt.newValue;});
    //        UIEnumFieldFunctions.Add("Weather", (ChangeEvent<Enum> evt) => { weatherSystem.weather = (Weather)evt.newValue; });
    //        UIEnumFieldFunctions.Add("MetallicRGB", (ChangeEvent<Enum> evt) => { metallicChannel = (ColorRGBA)evt.newValue; SetVector(metallicChannel, "_PBRMetallicChannel"); });
    //        UIEnumFieldFunctions.Add("SmoothnessRGB", (ChangeEvent<Enum> evt) => { smoothnessChannel = (ColorRGBA)evt.newValue; SetVector(smoothnessChannel, "_PBRSmothnessChannel"); });
    //        UIEnumFieldFunctions.Add("OcclusionRGB", (ChangeEvent<Enum> evt) => { occlusionChannel = (ColorRGBA)evt.newValue; SetVector(occlusionChannel, "_PBROcclusionChannel"); });
    //        UIEnumFieldFunctions.Add("DiffuseShadingMode", (ChangeEvent<Enum> evt) => 
    //        {
    //            diffuseShadingMode = (DiffuseShadingMode)evt.newValue;
    //            if (diffuseShadingMode == DiffuseShadingMode.CellBandsShading)
    //            {
    //                CellBandsShadingContainer.style.display = DisplayStyle.Flex;
    //                CelShadingContainer.style.display = DisplayStyle.None;
    //                RampShadingContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_CELLBANDSHADING"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_CELLBANDSHADING"), false);

    //            if (diffuseShadingMode == DiffuseShadingMode.CelShading)
    //            {
    //                CellBandsShadingContainer.style.display = DisplayStyle.None;
    //                CelShadingContainer.style.display = DisplayStyle.Flex;
    //                RampShadingContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_CELLSHADING"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_CELLSHADING"), false);

    //            if (diffuseShadingMode == DiffuseShadingMode.PBRShading)
    //            {
    //                CellBandsShadingContainer.style.display = DisplayStyle.None;
    //                CelShadingContainer.style.display = DisplayStyle.None;
    //                RampShadingContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_LAMBERTIAN"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_LAMBERTIAN"), false);

    //            if (diffuseShadingMode == DiffuseShadingMode.RampShading)
    //            {
    //                CellBandsShadingContainer.style.display = DisplayStyle.None;
    //                CelShadingContainer.style.display = DisplayStyle.None;
    //                RampShadingContainer.style.display = DisplayStyle.Flex;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_RAMPSHADING"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_RAMPSHADING"), false);
    //        });
    //        UIEnumFieldFunctions.Add("DiffuseShadingHairMode", (ChangeEvent<Enum> evt) =>
    //        {
    //            diffuseShadingHairMode = (DiffuseShadingHairMode)evt.newValue;

    //            if (diffuseShadingHairMode == DiffuseShadingHairMode.CelShading)
    //            {
    //                CelShadingContainer.style.display = DisplayStyle.Flex;
    //                RampShadingContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_CELLSHADING"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_CELLSHADING"), false);

    //            if (diffuseShadingHairMode == DiffuseShadingHairMode.RampShading)
    //            {
    //                RampShadingContainer.style.display = DisplayStyle.Flex;
    //                CelShadingContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_RAMPSHADING"), true);
    //            }
    //            else
    //            {
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_RAMPSHADING"), false);
    //            }

    //            if (diffuseShadingHairMode == DiffuseShadingHairMode.PBRShading)
    //            {
    //                RampShadingContainer.style.display = DisplayStyle.None;
    //                CelShadingContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_LAMBERTIAN"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_LAMBERTIAN"), false);
    //        });
    //        UIEnumFieldFunctions.Add("DiffuseShadingFaceMode", (ChangeEvent<Enum> evt) =>
    //        {
    //            diffuseShadingFaceMode = (DiffuseShadingFaceMode)evt.newValue;

    //            if (diffuseShadingFaceMode == DiffuseShadingFaceMode.CelShading)
    //            {
    //                CelFaceShadingContainer.style.display = DisplayStyle.Flex;
    //                CellBandsFaceShadingContainer.style.display = DisplayStyle.None;
    //                SDFFaceShadingContainer.style.display = DisplayStyle.None;
    //                RampFaceShadingContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_CELLSHADING"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_CELLSHADING"), false);

    //            if (diffuseShadingFaceMode == DiffuseShadingFaceMode.CellBandsShading)
    //            {
    //                CelFaceShadingContainer.style.display = DisplayStyle.None;
    //                CellBandsFaceShadingContainer.style.display = DisplayStyle.Flex;
    //                SDFFaceShadingContainer.style.display = DisplayStyle.None;
    //                RampFaceShadingContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_CELLSHADING"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_CELLSHADING"), false);

    //            if (diffuseShadingFaceMode == DiffuseShadingFaceMode.RampShading)
    //            {
    //                CelFaceShadingContainer.style.display = DisplayStyle.None;
    //                CellBandsFaceShadingContainer.style.display = DisplayStyle.None;
    //                SDFFaceShadingContainer.style.display = DisplayStyle.None;
    //                RampFaceShadingContainer.style.display = DisplayStyle.Flex;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_RAMPSHADING"), true);
    //            }
    //            else
    //            {
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_RAMPSHADING"), false);
    //            }

    //            if (diffuseShadingFaceMode == DiffuseShadingFaceMode.PBRShading)
    //            {
    //                CelFaceShadingContainer.style.display = DisplayStyle.None;
    //                CellBandsFaceShadingContainer.style.display = DisplayStyle.None;
    //                SDFFaceShadingContainer.style.display = DisplayStyle.None;
    //                RampFaceShadingContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_LAMBERTIAN"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_LAMBERTIAN"), false);

    //            if (diffuseShadingFaceMode == DiffuseShadingFaceMode.SDFFace)
    //            {
    //                CelFaceShadingContainer.style.display = DisplayStyle.None;
    //                CellBandsFaceShadingContainer.style.display = DisplayStyle.None;
    //                SDFFaceShadingContainer.style.display = DisplayStyle.Flex;
    //                RampFaceShadingContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_SDFFACE"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_SDFFACE"), false);
    //        });
    //        UIEnumFieldFunctions.Add("SpecularShadingMode", (ChangeEvent<Enum> evt) =>
    //        {
    //            specularShadingMode = (SpecularShadingMode)evt.newValue;
    //            if (specularShadingMode == SpecularShadingMode.Blinn_Phong)
    //            {
    //                PBR_GGXContainer.style.display = DisplayStyle.None;
    //                StylizedContainer.style.display = DisplayStyle.None;
    //                BlinnPhongContainer.style.display = DisplayStyle.Flex;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_BLINNPHONG"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_BLINNPHONG"), false);

    //            if (specularShadingMode == SpecularShadingMode.None)
    //            {
    //                PBR_GGXContainer.style.display = DisplayStyle.None;
    //                StylizedContainer.style.display = DisplayStyle.None;
    //                BlinnPhongContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_"), false);

    //            if (specularShadingMode == SpecularShadingMode.PBR_GGX)
    //            {
    //                PBR_GGXContainer.style.display = DisplayStyle.Flex;
    //                StylizedContainer.style.display = DisplayStyle.None;
    //                BlinnPhongContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_GGX"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_GGX"), false);

    //            if (specularShadingMode == SpecularShadingMode.Stylized)
    //            {
    //                PBR_GGXContainer.style.display = DisplayStyle.None;
    //                StylizedContainer.style.display = DisplayStyle.Flex;
    //                BlinnPhongContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_STYLIZED"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_STYLIZED"), false);
    //        });
    //        UIEnumFieldFunctions.Add("SpecularHairShadingMode", (ChangeEvent<Enum> evt) =>
    //        {
    //            specularHairShadingMode = (SpecularHairShadingMode)evt.newValue;
    //            if (specularHairShadingMode == SpecularHairShadingMode.AngleRing)
    //            {
    //                AnisotropyContainer.style.display = DisplayStyle.None;
    //                AngleRingContainer.style.display = DisplayStyle.Flex;
    //                PBR_GGXHairContainer.style.display = DisplayStyle.None;
    //                StylizedHairContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_ANGLERING"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_ANGLERING"), false);

    //            if (specularHairShadingMode == SpecularHairShadingMode.Anisotropy)
    //            {
    //                AnisotropyContainer.style.display = DisplayStyle.Flex;
    //                AngleRingContainer.style.display = DisplayStyle.None;
    //                PBR_GGXHairContainer.style.display = DisplayStyle.None;
    //                StylizedHairContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_KAJIYAHAIR"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_KAJIYAHAIR"), false);

    //            if (specularHairShadingMode == SpecularHairShadingMode.None)
    //            {
    //                AnisotropyContainer.style.display = DisplayStyle.None;
    //                AngleRingContainer.style.display = DisplayStyle.None;
    //                PBR_GGXHairContainer.style.display = DisplayStyle.None;
    //                StylizedHairContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_"), false);

    //            if (specularHairShadingMode == SpecularHairShadingMode.PBR_GGX)
    //            {
    //                AnisotropyContainer.style.display = DisplayStyle.None;
    //                AngleRingContainer.style.display = DisplayStyle.None;
    //                PBR_GGXHairContainer.style.display = DisplayStyle.Flex;
    //                StylizedHairContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_GGX"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_GGX"), false);

    //            if (specularHairShadingMode == SpecularHairShadingMode.Stylized)
    //            {
    //                AnisotropyContainer.style.display = DisplayStyle.None;
    //                AngleRingContainer.style.display = DisplayStyle.None;
    //                PBR_GGXHairContainer.style.display = DisplayStyle.None;
    //                StylizedHairContainer.style.display = DisplayStyle.Flex;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_STYLIZED"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_STYLIZED"), false);
    //        });
    //        UIEnumFieldFunctions.Add("RimMode", (ChangeEvent<Enum> evt) =>
    //        {
    //            rimMode = (RimMode)evt.newValue;
    //            if (rimMode == RimMode.None)
    //            {
    //                FresnelRimContainer.style.display = DisplayStyle.None;
    //                ScreenSpaceRimContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_"), false);

    //            if (rimMode == RimMode.FresnelRim)
    //            {
    //                FresnelRimContainer.style.display = DisplayStyle.Flex;
    //                ScreenSpaceRimContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_FRESNELRIM"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_FRESNELRIM"), false);

    //            if (rimMode == RimMode.ScreenSpaceRim)
    //            {
    //                FresnelRimContainer.style.display = DisplayStyle.None;
    //                ScreenSpaceRimContainer.style.display = DisplayStyle.Flex;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_SCREENSPACERIM"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_SCREENSPACERIM"), false);
    //        });
    //        UIEnumFieldFunctions.Add("RimHairMode", (ChangeEvent<Enum> evt) =>
    //        {
    //            rimMode = (RimMode)evt.newValue;
    //            if (rimMode == RimMode.None)
    //            {
    //                FresnelHairRimContainer.style.display = DisplayStyle.None;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_"), false);

    //            if (rimMode == RimMode.FresnelRim)
    //            {
    //                FresnelHairRimContainer.style.display = DisplayStyle.Flex;
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_FRESNELRIM"), true);
    //            }
    //            else
    //                materialSelected.SetKeyword(new LocalKeyword(materialSelected.shader, "_FRESNELRIM"), false);
    //        });
    //        UIEnumFieldFunctions.Add("Shaders", (ChangeEvent<Enum> evt) =>
    //        {
    //            fernShader = (FernShaders)evt.newValue;
    //            if (fernShader == FernShaders.Standard)
    //            {
    //                materialSelected.shader = standardShader;
    //            }
    //            if (fernShader == FernShaders.Hair)
    //            {
    //                materialSelected.shader = hairShader;
    //            }
    //            if (fernShader == FernShaders.Face)
    //            {
    //                materialSelected.shader = faceShader;
    //            }
    //        }); 
    //        UIEnumFieldFunctions.Add("KeyframeActionMode", (ChangeEvent<Enum> evt) =>
    //            {
    //                KeyframeAction = (KeyframeActionState)evt.newValue;
    //            });

    //        //IntegerField
    //        UIIntegerFieldFunctions.Add("FrameIndex", (ChangeEvent<int> evt) =>{keyframeIndex = evt.newValue; });

    //        //FloatField
    //        UIFloatFieldFunctions.Add("FrameValue", (ChangeEvent<float> evt) => { keyframeValue = evt.newValue; });
    //        UIFloatFieldFunctions.Add("NormapScale", (ChangeEvent<float> evt) => { NormapScale = evt.newValue; });
    //    }

    //    #region helpFunctions

    //    private void SetTexture(string keyword)
    //    {
    //        ButtonTexFolder.OpenBrowser(coroutineExecuter);
    //        StartCoroutine(MaterialUtils.GetTextureHTTP(materialSelected, Path.Combine("file://", TexPath), keyword));
    //    }

    //    private void SetVector(ColorRGBA channel, string keyword)
    //    {
    //        if(channel == ColorRGBA.R)
    //            materialSelected.SetVector(keyword, new Vector4(1, 0, 0, 0));
    //        if (channel == ColorRGBA.G)
    //            materialSelected.SetVector(keyword, new Vector4(0, 1, 0, 0));
    //        if (channel == ColorRGBA.B)
    //            materialSelected.SetVector(keyword, new Vector4(0, 0, 1, 0));
    //        if (channel == ColorRGBA.A)
    //            materialSelected.SetVector(keyword, new Vector4(0, 0, 1, 0));
    //        if (channel == ColorRGBA.Average)
    //            materialSelected.SetVector(keyword, new Vector4(0.33f, 0.33f, 0.33f, 1));
    //        if (channel == ColorRGBA.Luminance)
    //            materialSelected.SetVector(keyword, new Vector4(0.22f, 0.707f, 0.071f, 0.0f));
    //    }

    //    public enum DayTime
    //    {
    //        Daytime, Dusk, Nightime
    //    }

    //    public enum Weather
    //    {
    //        Sunny, Cloudy, Rainy, Snowy, Blizzard, Lightening
    //    }

    //    public enum ColorRGB
    //    {
    //        R, G, B
    //    }

    //    public enum ColorRGBA
    //    {
    //        R, G, B, A, Average, Luminance
    //    }

    //    public enum DiffuseShadingMode
    //    {
    //        CelShading, RampShading, CellBandsShading, PBRShading
    //    }
    //    public enum DiffuseShadingHairMode
    //    {
    //        CelShading, RampShading, PBRShading
    //    }
    //    public enum DiffuseShadingFaceMode
    //    {
    //        CelShading, CellBandsShading, RampShading, PBRShading, SDFFace
    //    }

    //    public enum SpecularShadingMode
    //    {
    //        None, PBR_GGX, Stylized, Blinn_Phong
    //    }

    //    public enum SpecularHairShadingMode
    //    {
    //        None, PBR_GGX, Stylized, Anisotropy, AngleRing
    //    }

    //    public enum RimMode
    //    {
    //        None, FresnelRim, ScreenSpaceRim
    //    }
    //    public enum RimHairMode
    //    {
    //        None, FresnelRim
    //    }

    //    public enum KeyframeFunctions
    //    {
    //        Modify, Add, Remove
    //    }

    //    public enum FernShaders
    //    {
    //        Standard, Face, Hair
    //    }

    //    public enum KeyframeActionState
    //    {
    //        Add, Remove, Modify
    //    }

    //    #endregion
    //}
}
