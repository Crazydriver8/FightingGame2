using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public enum AnimationFlow{
	UFEEngine,
	UnityEngine
}

#if UFE_BASIC
public enum AnimationType{
	Legacy
}
#else
public enum AnimationType {
    Legacy,
    Mecanim
}
#endif

public enum BlockType{
	None,
	HoldBack,
	AutoBlock,
	HoldButton1,
	HoldButton2,
	HoldButton3,
	HoldButton4,
	HoldButton5,
	HoldButton6,
	HoldButton7,
	HoldButton8,
	HoldButton9,
	HoldButton10,
	HoldButton11,
	HoldButton12
}

public enum BodyPart {
	none,
	head,
	upperTorso,
	lowerTorso,
	leftUpperArm,
	rightUpperArm,
	leftForearm,
	rightForearm,
	leftHand,
	rightHand,
	leftThigh,
	rightThigh,
	leftCalf,
	rightCalf,
	leftFoot,
	rightFoot,
	root,
	custom1,
	custom2,
	custom3,
	custom4,
	custom5,
	custom6,
	custom7,
	custom8,
	custom9
}

public enum NetworkService {
	P2P,
	UFEOnline,
    //Playfab,
    Unity
}

public enum CollisionType {
    bodyCollider,
    hitCollider,
    noCollider,
    throwCollider
}

public enum ComboDisplayMode{
	ShowDuringComboExecution,
	ShowAfterComboExecution,
}

public enum GameMode{
	None,
	StoryMode,
	VersusMode,
	TrainingRoom,
	NetworkGame,
}

public enum Gender {
	Unknown,
	Male,
	Female
}

public enum HitBoxType {
	high,
	low
}

public enum HitBoxShape {
	circle,
	rectangle
}

public enum ExecutionBufferType{
	OnlyMoveLinks,
	AnyMove,
	NoBuffer
}

public enum ParryType{
	None,
	TapBack,
	TapForward,
	TapButton1,
	TapButton2,
	TapButton3,
	TapButton4,
	TapButton5,
	TapButton6,
	TapButton7,
	TapButton8,
	TapButton9,
	TapButton10,
	TapButton11,
	TapButton12
}

public enum ParryStunType{
	Fixed,
	BlockStunPercentage
}

public enum Player { 
	Player1,
	Player2
}

public enum Side {
	Left,
	Right
}

public enum Sizes{
	None,
	Small,
	Medium,
	High
}

public enum AirJuggleDeteriorationType{
	ComboHits,
	AirHits
}

public enum AirRecoveryType {
    AllowMoves,
    CantMove,
    DontRecover
}

#if !UFE_BASIC
public enum InputManagerType{
	UnityInputManager,
	cInput,
	ControlFreak
}
#else
public enum InputManagerType{
	UnityInputManager,
	cInput
}
#endif

public enum AIEngine{
	RandomAI,
	FuzzyAI
}

public enum AIBehavior{
	Any,
	VeryDefensive,
	Defensive,
	Balanced,
	Aggressive,
	VeryAggressive
}

public enum AIDifficultyLevel{
	VeryEasy,
	Easy,
	Normal,
	Hard,
	VeryHard,
	Impossible
}

public enum LifeBarTrainingMode{
	Refill,
	Infinite,
	Normal
}

public enum LobbyMatchCreationSystem{
	ManualRoomCreation,
	MatchMaking
}

public enum MatchMakingFilterType {
    Range,
    Equal,
    Different,
    HigherThen,
    LowerThen
}

public enum ServerVariableUpdateType {
    ELO,
    Set,
    Increment
}

public enum ServerVariableType {
    Float,
    Integer,
    String,
    Boolean
}

public enum UFEBoolean {
    TRUE,
    FALSE
}

public enum HitEffectSpawnPoint {
    StrikingHurtBox,
    StrokeHitBox,
    InBetween
}

[System.Serializable]
public class AIInstructionsSet: ICloneable {
	public ScriptableObject aiInfo;
	public AIBehavior behavior;
	
	public object Clone() {
		return CloneObject.Clone(this);
	}
}

[System.Serializable]
public class AIDifficultySettings: ICloneable {
	public AIDifficultyLevel difficultyLevel;
	public bool overrideTimeBetweenDecisions;
	public float timeBetweenDecisions = 0;
	public bool overrideTimeBetweenActions;
	public float timeBetweenActions = 0.05f;
	public bool overrideAggressiveness;
	public float aggressiveness = 0.5f;
	public bool overrideRuleCompliance;
	public float ruleCompliance = .9f;
	public bool overrideComboEfficiency;
	public float comboEfficiency = 1f;
	public AIBehavior startupBehavior;

	public object Clone() {
		return CloneObject.Clone(this);
	}
}

[System.Serializable]
public class DebugOptions {
    public bool startGameImmediately;
    public bool skipLoadingScreen;
    public bool trainingMode;
    public bool debugMode;
    public bool trainingModeDebugger;
    public bool preloadedObjects;

    public bool networkToggle;
    public bool connectionLog = true;
    public bool ping = true;
    public bool frameDelay = true;
    public bool currentLocalFrame = true;
    public bool currentNetworkFrame = true;

    public CharacterDebugInfo p1DebugInfo;
    public CharacterDebugInfo p2DebugInfo;
}

[System.Serializable]
public class TrainingModeOptions {
    public bool inputInfo;
    public bool freezeTime;
    public float p1StartingLife = 100f;
    public float p2StartingLife = 100f;
    public LifeBarTrainingMode p1Life;
    public LifeBarTrainingMode p1Gauge;
    public LifeBarTrainingMode p2Life;
    public LifeBarTrainingMode p2Gauge;
    public float refillTime = 3f;
}

[System.Serializable]
public class CharacterDebugInfo {
    public bool toggle;
    public bool currentMove = true;
    public bool position = true;
    public bool lifePoints = true;
    public bool currentState;
    public bool currentSubState;
    public bool stunTime = true;
    public bool comboHits = true;
    public bool comboDamage = true;
    public bool inputs = true;
    public bool buttonSequence;
    public bool aiWeightList;
}

[System.Serializable]
public class AIOptions {
	public AIEngine engine;
	
	// Random AI Engine
	public bool attackWhenEnemyIsDown = false;
	public bool moveWhenEnemyIsDown = false;
	public float inputFrequency = .3f;
	public bool behaviourToggle;
	public AIDistanceBehaviour[] distanceBehaviour = new AIDistanceBehaviour[0];
	
	
	// Fuzzy AI Engine
	public bool multiCoreSupport = true;
	public bool persistentBehavior = false;
	public bool difficultyToggle;
	public AIDifficultySettings[] difficultySettings = new AIDifficultySettings[0];

	[HideInInspector] public AIDifficultyLevel selectedDifficultyLevel = AIDifficultyLevel.Normal;
	[HideInInspector] public AIDifficultySettings selectedDifficulty;
}

[System.Serializable]
public class CameraOptions {
	public Vector3 initialDistance;
	public Vector3 initialRotation;
	public float initialFieldOfView;
	public bool followJumpingCharacter;
	public float movementSpeed = 15;
	public float minZoom = 38;
	public float maxZoom = 54;
	public float maxDistance = 22;
	public bool enableLookAt;
	public float rotationSpeed = 20;
	public float heightOffSet = 4;
	public Vector3 rotationOffSet = new Vector3(0, 4, 0);
}

[System.Serializable]
public class CharacterRotationOptions {
	public bool autoMirror = true;
	public bool rotateWhileJumping = false;
	public bool rotateOnMoveOnly = false;
	public bool fixRotationWhenStunned = false;
	public bool fixRotationWhenBlocking = true;
	public bool fixRotationOnHit = true;
	public float rotationSpeed = 10;
	public float mirrorBlending = .1f;
}

[System.Serializable]
public class ComboOptions {
	public ComboDisplayMode comboDisplayMode;
	public Sizes hitStunDeterioration;
	public Sizes damageDeterioration;
	public Sizes airJuggleDeterioration;
	public float minHitStun = 1;
	public float minDamage = 5;
	public float minPushForce = 5;
    public int maxConsecutiveCrumple = 1;
	public AirJuggleDeteriorationType airJuggleDeteriorationType;
    public bool neverAirRecover = false;
    public AirRecoveryType airRecoveryType = AirRecoveryType.CantMove;
	public bool resetFallingForceOnHit = true;
	public int maxCombo = 99;
	public float knockBackMinForce = 0;
	public bool neverCornerPush;
	public bool fixJuggleWeight = true;
	public float juggleWeight = 200;
}

[System.Serializable]
public class BounceOptions {
	public Sizes bounceForce;
	public GameObject bouncePrefab;
	public float bounceKillTime = 2;
	public float minimumBounceForce = 30;
	public float maximumBounces = 2;
	public bool bounceHitBoxes = true;
	public bool shakeCamOnBounce = true;
	public float shakeDensity = .6f;
	public AudioClip bounceSound;
}

[System.Serializable]
public class BlockOptions {
	public BlockType blockType;
    public bool allowAirBlock;
    public bool ignoreAppliedForceBlock;

	public GameObject blockPrefab;
	public float blockKillTime;
	public AudioClip blockSound;
	public bool overrideBlockHitEffects;
	public HitTypeOptions blockHitEffects;



	public ParryType parryType;
	public float parryTiming;
	public ParryStunType parryStunType;
	public int parryStunFrames = 10;
	
	public GameObject parryPrefab;
	public float parryKillTime;
	public AudioClip parrySound;
	public bool overrideParryHitEffects;
	public HitTypeOptions parryHitEffects;


	public Color parryColor;
	public bool allowAirParry;
	public bool highlightWhenParry;
	public bool resetButtonSequence;
	public bool easyParry;
	public bool ignoreAppliedForceParry;
	public Sizes blockPushForce; // NOT DONE
	public ButtonPress[] pushBlockButtons; // NOT DONE
}

[System.Serializable]
public class CanvasScalerInformation{
	public float defaultSpriteDPI = 96f;
	public float fallbackScreenDPI = 96f;
	public float matchWidthOrHeight = 0f;
	public CanvasScaler.Unit physicalUnit = CanvasScaler.Unit.Points;
	public float referencePixelsPerUnit = 100f;
	public Vector2 referenceResolution = new Vector2(1920f, 1080f);
	public float scaleFactor = 1f;
	public CanvasScaler.ScreenMatchMode screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
	public CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

	//---------------------------------------------------------------------------------------------------------
	// We use comment the next line because we use a "Screen Space - Overlay" canvas
	// and the "dynamicPixelsPerUnit" property is only used in "World Space" Canvas.
	//---------------------------------------------------------------------------------------------------------
	//public float dynamicPixelsPerUnit = 100f;
}

[System.Serializable]
public class GameGUI{
	public bool hasGauge = true;
    public float screenFadeDuration = 2f;
    public float gameFadeDuration = 2f;
    public Color screenFadeColor = Color.black;
    public Color gameFadeColor = Color.black;
	public bool useCanvasScaler = false;
	public CanvasScalerInformation canvasScaler = new CanvasScalerInformation();

	public IntroScreen introScreen;
	public MainMenuScreen mainMenuScreen;
	public OptionsScreen optionsScreen;
    public SkillTreeScreen skillTreeScreen;
	public CreditsScreen creditsScreen;
	public PauseScreen pauseScreen;

	public CharacterSelectionScreen characterSelectionScreen;
	public StageSelectionScreen stageSelectionScreen;
	public LoadingBattleScreen loadingBattleScreen;
	public BattleGUI battleGUI;

	public StoryModeContinueScreen storyModeContinueScreen;
	public StoryModeScreen storyModeGameOverScreen;
	public StoryModeScreen storyModeCongratulationsScreen;

	public VersusModeScreen versusModeScreen;
	public VersusModeAfterBattleScreen versusModeAfterBattleScreen;
	
	public ConnectionLostScreen connectionLostScreen;
	public HostGameScreen hostGameScreen;
	public JoinGameScreen joinGameScreen;
	public NetworkGameScreen networkGameScreen;
	public BluetoothGameScreen bluetoothGameScreen;
}

[System.Serializable]
public class KnockDownOptions {
	public SubKnockdownOptions air;
	public SubKnockdownOptions high;
	public SubKnockdownOptions highLow;
	public SubKnockdownOptions sweep;
    public SubKnockdownOptions crumple;
    public SubKnockdownOptions wallbounce;
}

[System.Serializable]
public class SubKnockdownOptions {
	public float knockedOutTime = 2;
	public float standUpTime = .6f;
	public int hideHitBoxesOnFrame = 10;
    public bool hideHitBoxes;
	public bool editorToggle;
	public bool hasQuickStand;
	public Vector2 predefinedPushForce;
	public ButtonPress[] quickStandButtons = new ButtonPress[0];
	public float minQuickStandTime;
	public bool hasDelayedStand;
	public ButtonPress[] delayedStandButtons = new ButtonPress[0];
	public float maxDelayedStandTime;
}

[System.Serializable]
public class HitTypeOptions {
	public GameObject hitParticle;
	public float killTime;
    public AudioClip hitSound;
    public HitEffectSpawnPoint spawnPoint = HitEffectSpawnPoint.StrokeHitBox;
	public float freezingTime;
	public float animationSpeed = .1f;
	public bool autoHitStop = true;
	public float hitStop = .1f;
	public bool shakeCharacterOnHit = true;
	public bool shakeCameraOnHit = true;
	public float shakeDensity = .8f;
	public bool editorToggle;
}

[System.Serializable]
public class HitOptions {
	public bool resetAnimationOnHit = true;
	public bool useHitStunDeceleration = true;
	public HitTypeOptions weakHit;
	public HitTypeOptions mediumHit;
	public HitTypeOptions heavyHit;
	public HitTypeOptions crumpleHit;
	public HitTypeOptions customHit1;
	public HitTypeOptions customHit2;
	public HitTypeOptions customHit3;
}

[System.Serializable]
public class InputOptions {
	public InputManagerType inputManagerType;
	public bool cInputAllowDuplicates = false;
	public float cInputGravity = 3;
	public float cInputSensitivity = 3;
	public float cInputDeadZone = 0.001f;
	public GUISkin cInputSkin = null;
	public GameObject controlFreakPrefab = null;
    public float controlFreakDeadZone = 0.5f;
    public bool forceDigitalInput = true;
	public ButtonPress confirmButton;
	public ButtonPress cancelButton;
}

[System.Serializable]
public class StageOptions: ICloneable {
	public string stageName;
	public Texture2D screenshot;
	public GameObject prefab;
	public AudioClip music;
	public float groundFriction = 100;
	public float leftBoundary = -38;
	public float rightBoundary = 38;

	public object Clone() {
		return CloneObject.Clone(this);
	}
}

[System.Serializable]
public class LanguageOptions: ICloneable {
	public string languageName = "English";
	public string start = "Start";
	public string options = "Options";
	public string credits = "Credits";
	public string selectYourCharacter = "Select Your Character";
	public string selectYourStage = "Select Your Stage";
	public string round = "Round %round%";
	public string finalRound = "Final Round";
	public string fight = "Fight!";
	public string firstHit = "First Hit!";
	public string combo = "%number% hit combo!";
	public string parry = "Parry!";
	public string counterHit = "Counter!";
	public string victory = "%character% wins!";
	public string timeOver = "Time Over";
	public string perfect = "Perfect!";
	public string rematch = "Rematch";
	public string quit = "Quit";
	public string ko = "K.O.";
	public bool defaultSelection;

	public object Clone() {
		return CloneObject.Clone(this);
	}
}

[System.Serializable]
public class AnnouncerOptions {
	[HideInInspector]public string announcerName = string.Empty;
	public AudioClip round1;
	public AudioClip round2;
	public AudioClip round3;
	public AudioClip otherRounds;
	public AudioClip finalRound;
	public AudioClip fight;
	public AudioClip player1Wins;
	public AudioClip player2Wins;
	public AudioClip perfect;
	public AudioClip firstHit;
	public AudioClip counterHit;
	public AudioClip parry;
	public AudioClip timeOver;
	public AudioClip ko;
	//public bool combosToggle;
	public ComboAnnouncer[] combos;
}

[System.Serializable]
public class ComboAnnouncer: ICloneable {
	public AudioClip audio;
	public int hits;

	public object Clone() {
		return CloneObject.Clone(this);
	}
}

[System.Serializable]
public class CounterHitOptions {
	public bool startUpFrames = true;
	public bool activeFrames = false;
	public bool recoveryFrames = false;
	public float damageIncrease = 10;
	public float hitStunIncrease = 50;
	public AudioClip sound;
}

[System.Serializable]
public class GUIBarOptions {
	public bool editorToggle;
	public bool previewToggle;
	public bool flip;
	public Texture2D backgroundImage;
	public Color backgroundColor;
	public Texture2D fillImage;
	public Color fillColor;
	public Rect backgroundRect;
	public Rect fillRect;
	public GameObject bgPreview;
	public GameObject fillPreview;
}

[System.Serializable]
public class InputReferences: ICloneable {
	// Common Parameters
	public InputType inputType;
	public string inputButtonName;
	public ButtonPress engineRelatedButton;
	
	// Input Manager parameters
	public string joystickAxisName;
	
	// cInput parameters
	public string cInputPositiveKeyName;
	public string cInputPositiveDefaultKey;
	public string cInputPositiveAlternativeKey;
	
	public string cInputNegativeKeyName;
	public string cInputNegativeDefaultKey;
	public string cInputNegativeAlternativeKey;
	
	// Input Viewer
	public Texture2D inputViewerIcon1;
	public Texture2D inputViewerIcon2;
	public Texture2D activeIcon;
	
	public float heldDown{get; set;}
	
	public object Clone() {
		return CloneObject.Clone(this);
	}
}

[System.Serializable]
public class RoundOptions {
	public int totalRounds = 3;
	public bool hasTimer = true;
	public float timer = 99;
	public float timerSpeed = 100;
	public float p1XPosition = -5;
	public float p2XPosition = 5;
	public float endGameDelay = 4;
	public float newRoundDelay = 1;
	public AudioClip victoryMusic;
	public bool resetLifePoints = true;
	public bool resetPositions = true;
	public bool allowMovement = true;
	public bool slowMotionKO = true;
	public bool cameraZoomKO = true;
	public bool freezeCamAfterOutro = true;
}

[System.Serializable]
public class CharacterStories : DGP.Util.Collections.SerializableDictionary<int, CharacterStory>{}

[System.Serializable]
public class StoryMode{
	public bool useSameStoryForAllCharacters;
	public bool canCharactersFightAgainstThemselves;
	public CharacterStory defaultStory;

	public List<int> selectableCharactersInStoryMode = new List<int>();
	public List<int> selectableCharactersInVersusMode = new List<int>();
	public CharacterStories characterStories = new CharacterStories();
}

[System.Serializable]
public class StoryModeBattle{
	public StoryModeScreen conversationBeforeBattle;
	public StoryModeScreen conversationAfterBattle;
	public int opponentCharacterIndex;
	public List<int> possibleStagesIndexes = new List<int>();
}

public class StoryModeInfo{
	// The information about the character story
	public CharacterStory characterStory = null;

	// Whether the character can fight against himself in Story Mode
	public bool canFightAgainstHimself = false;

	// The index of the current "group"
	public int currentGroup = 0;

	// The index of the current "battle" in the current "group"
	public int currentBattle = 0;

	// The information about the current battle
	public StoryModeBattle currentBattleInformation = null;

	// The indexes of the characters that have been defeated in the current "group".
	// It's used only if the player must fight the opponents in a group in random order.
	public HashSet<int> defeatedOpponents = new HashSet<int>();
}

[System.Serializable]
public class FightsGroup{
	public int maxFights = 4; // maxFights is only used when mode == FightsGroupMode.FightAgainstSeveralRandomOpponents
	public FightsGroupMode mode = FightsGroupMode.FightAgainstAllOpponentsInTheGroupInRandomOrder;
	public string name = string.Empty;
	public StoryModeBattle[] opponents = new StoryModeBattle[0];
	public bool showOpponentsInEditor;
}

public enum FightsGroupMode{
	FightAgainstSeveralOpponentsInTheGroupInRandomOrder,
	FightAgainstAllOpponentsInTheGroupInRandomOrder,
	FightAgainstAllOpponentsInTheGroupInTheDefinedOrder
}

[System.Serializable]
public class CharacterStory{
	public StoryModeScreen opening;
	public StoryModeScreen ending;
	public FightsGroup[] fightsGroups = new FightsGroup[0];
	public bool showStoryInEditor;
}

[System.Serializable]
public class AIDistanceBehaviour: ICloneable {
	public CharacterDistance characterDistance;
	public int proximityRangeBegins = 0;
	public int proximityRangeEnds = 100;

	public float movingForwardProbability = .5f;
	public float movingBackProbability = .5f;
	public float jumpingProbability = .5f;
	public float crouchProbability = .5f;
	public float attackProbability = .5f;
	
	public object Clone() {
		return CloneObject.Clone(this);
	}
}

[System.Serializable]
public class NetworkUserData: ICloneable {
    public string variableName;
    public ServerVariableType variableType;
    public float floatValue;
    public int intValue;
    public string stringValue;
    public bool boolValue;
    public ServerVariableUpdateType variableUpdateType;
    public MatchMakingFilterType matchMakingFilterType;

    public UFEBoolean ufeBoolean;

    public object Clone() {
        return CloneObject.Clone(this);
    }
}

[System.Serializable]
public class LobbyOptions: ICloneable {
    public string lobbyName;
    public LobbyMatchCreationSystem matchMakingType;
    public GameMode gameMode;
    public bool allowPrivateRooms;
    public bool matchMakingToggle;
    public bool winnerToggle;
    public bool loserToggle;
    public NetworkUserData[] matchMakingUserData = new NetworkUserData[0];
    public NetworkUserData[] winnerUserData = new NetworkUserData[0];
    public NetworkUserData[] loserUserData = new NetworkUserData[0];

    public object Clone() {
        return CloneObject.Clone(this);
    }
}

[System.Serializable]
public class NetworkOptions {
    //general options
	public bool fakeNetwork;
    public bool forceAnimationControl;
    public bool applyFrameDelayOffline;
    public bool disconnectOnDesync;

    //service
    public NetworkService networkService;
    public string authKey;
    public string titleId;
    public string photonKey;
    public LobbyOptions[] lobbies = new LobbyOptions[] { new LobbyOptions() };

    public int port = 1337;


    //netcode
	public bool automaticFrameDelay = false;
    public int maxFrameDelay = 10;
	public int defaultFrameDelay = 3;

    public bool userRollBack;
    public int rollBackFrames;

}


[System.Serializable]
public class GlobalInfo: ScriptableObject {

	#region public instance fields
	public float version = 0;
	public LanguageOptions selectedLanguage;
	public CharacterInfo player1Character;
	public CharacterInfo player2Character;
	public CharacterInfo p1CharStorage;
	public CharacterInfo p2CharStorage;
    public StageOptions selectedStage;
	public bool p1CPUControl;
	public bool p2CPUControl;
	public string gameName;


	//-----------------------------------------------------------------------------------------------------------------
	public GameGUI gameGUI;
	public StoryMode storyMode;
	//-----------------------------------------------------------------------------------------------------------------


	public int fps = 60;
	public float gameSpeed = 1;
	public int executionBufferTime = 10;
	public ExecutionBufferType executionBufferType;
    public int plinkingDelay = 1;

    public float preloadingTime = 1f;
    public bool preloadCharacter1 = true;
    public bool preloadCharacter2 = true;
    public bool preloadStage = true;
    public bool preloadHitEffects = true;
    public bool warmAllShaders = true;

	public float gravity = .37f;
	public bool detect3D_Hits;
	public bool runInBackground;
	public LanguageOptions[] languages = new LanguageOptions[]{new LanguageOptions()};
	public CameraOptions cameraOptions;
	public CharacterRotationOptions characterRotationOptions;
	public RoundOptions roundOptions;
	public BounceOptions groundBounceOptions;
	public BounceOptions wallBounceOptions;
	public CounterHitOptions counterHitOptions;
	public ComboOptions comboOptions;
	public BlockOptions blockOptions; // NOT DONE
	public KnockDownOptions knockDownOptions; // NOT DONE
	public HitOptions hitOptions;
	
	public InputReferences[] player1_Inputs = new InputReferences[0]; // Reference to Unity's InputManager to UFE's keys
	public InputReferences[] player2_Inputs = new InputReferences[0]; // Reference to Unity's InputManager to UFE's keys
	public InputOptions inputOptions = new InputOptions();
	
	public StageOptions[] stages = new StageOptions[0];
	public CharacterInfo[] characters = new CharacterInfo[0];
	public DebugOptions debugOptions = new DebugOptions();
	public TrainingModeOptions trainingModeOptions = new TrainingModeOptions();
	public AIOptions aiOptions = new AIOptions();
    public NetworkOptions networkOptions = new NetworkOptions();

	public bool music = true;
	public float musicVolume = 1f;
	public bool soundfx = true;
	public float soundfxVolume = 1f;

	public int currentRound{get; set;}
	public bool lockInputs{get; set;}
	public bool lockMovements{get; set;}
	#endregion

	#region public instance methods
	public virtual void ValidateStoryModeInformation(){
		// First, check that every character index in Story Mode is valid
		for (int i = this.storyMode.selectableCharactersInStoryMode.Count - 1; i >= 0; --i){
			int character = this.storyMode.selectableCharactersInStoryMode[i];

			if (character < 0 || character >= this.characters.Length){
				this.storyMode.characterStories.Remove(character);
				this.storyMode.selectableCharactersInStoryMode.RemoveAt(i);
			}else if (!this.storyMode.characterStories.ContainsKey(character)){
				this.storyMode.characterStories[character] = new CharacterStory();
			}
		}

		// Then check that every character index in Versus Mode is valid
		for (int i = this.storyMode.selectableCharactersInVersusMode.Count - 1; i >= 0; --i){
			int character = this.storyMode.selectableCharactersInVersusMode[i];
			if (character < 0 || character >= this.characters.Length){
				this.storyMode.selectableCharactersInVersusMode.RemoveAt(i);
			}
		}

		// Finally, check that every character and stage index are valid in the Character Stories
		this.ValidateCharacterStory(this.storyMode.defaultStory);
		foreach(CharacterStory story in this.storyMode.characterStories.Values){
			this.ValidateCharacterStory(story);
		}
	}
	#endregion

	#region protected instance methods
	protected virtual void ValidateCharacterStory(CharacterStory story){
		if (story != null && story.fightsGroups != null){
			foreach(FightsGroup group in story.fightsGroups){
				List<StoryModeBattle> battles = new List<StoryModeBattle>(group.opponents);

				for (int i = battles.Count - 1; i >= 0; --i){
					StoryModeBattle battle = battles[i];

					if (battle.opponentCharacterIndex < 0 || battle.opponentCharacterIndex >= this.characters.Length){
						battles.RemoveAt(i);
					}else{
						for (int j = battle.possibleStagesIndexes.Count - 1; j >= 0; --j){
							int stageIndex = battle.possibleStagesIndexes[j];

							if (stageIndex < 0 || stageIndex >= this.stages.Length){
								battle.possibleStagesIndexes.RemoveAt(j);
							}
						}

						if (battle.possibleStagesIndexes.Count == 0 && this.stages.Length > 0){
							battle.possibleStagesIndexes.Add(i % this.stages.Length);
						}
					}
				}

				group.opponents = battles.ToArray();
			}
		}
	}
	#endregion
}

[System.Serializable]
public static class CloneObject{
	public static object objCopy;
	
	public static object Clone(object target){
		return ReflectionClone(target);
	}
	
	public static object Clone(object target, bool serialized){
		if (serialized) return SerializedClone(target);
		return ReflectionClone(target);
	}

	public static object SerializedClone(object target){
		if (target == null) return null;

		using (Stream objectStream = new MemoryStream()) {
			IFormatter formatter = new BinaryFormatter();
			formatter.Serialize(objectStream, target);
			objectStream.Seek(0, SeekOrigin.Begin);
			return (object)formatter.Deserialize(objectStream);
		}
	}

	public static object[] ReflectionCloneArray(object[] target){
		object[] arrayObj = (object[]) Array.CreateInstance(target.GetType().GetElementType(), target.Length);

		for (int i = 0; i < target.Length; i ++){
			
			if (target[i] == null 
			    || target[i].GetType().IsEnum
			    || target[i].GetType().IsValueType
			    || target[i].GetType().IsGenericType
			    || target[i].GetType().Equals(typeof(String)) 
			    || target[i].GetType().IsSubclassOf(typeof(ScriptableObject))){
				
				// If its a simple element, use shallow copy
				arrayObj[i] = target[i];
			}else{
				// If its a complex interface, go deeper into recursion
				arrayObj[i] = ReflectionClone(target[i]);
			}
		}

		return arrayObj;
	}

	public static object ReflectionClone(object target){
		Type typeSource = target.GetType();

		// If its an array, identify and recurse each element
		if (typeSource.IsArray){
			return (object) ReflectionCloneArray((object[])target);
		}

		object newObj = Activator.CreateInstance(typeSource);
		FieldInfo[] fields = typeSource.GetFields();

		foreach (FieldInfo field in fields){
			object fieldValue = field.GetValue(target);

			if (fieldValue == null 
			    || field.FieldType.IsEnum
			    || field.FieldType.IsValueType
			    || field.FieldType.Equals(typeof(String)) 
			    || field.FieldType.GetInterface("ICloneable", true ) == null
			    || field.FieldType.GetInterface("ScriptableObject", true ) != null
			    || field.FieldType.IsSubclassOf(typeof(ScriptableObject))){
				// If its a simple element, use shallow copy
				field.SetValue(newObj, fieldValue);
			}else{
				// If its a complex interface, go deeper into recursion
				field.SetValue(newObj, ReflectionClone(fieldValue));

			}
		}
		return newObj;
	}
}