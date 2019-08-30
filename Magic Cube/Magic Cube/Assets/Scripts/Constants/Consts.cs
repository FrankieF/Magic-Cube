public class Consts
{
    // Prefabs
    public static string CUBE_PREFAB_NAME = "Cube{0}";
    public static string CUBE_PREFAB_PATH = "Prefabs/" + CUBE_PREFAB_NAME;

    // Text
    public static string RETURN_TO_MENU_TEXT = "Are you sure you want to return to the menu? Progress will be saved.";
    public static string PLAYER_WON_TEXT = "You won, congratulations! It took you {0} to solve it.";
    public static string TIMER_MM_SS_FORMAT_TEXT = "{0:0}:{1:00}";
    
    // Layers
    public const int CUBE_SELECT_LAYER_MASK = 8;
    public const int RIGHT_OF_CUBE_LAYER = 10;
    public const int FRONT_OF_CUBE_LAYER = 11;
    public const int TOP_OF_CUBE_LAYER = 12;
    public const int LEFT_OF_CUBE_LAYER = 13;
    public const int BOTTOM_OF_CUBE_LAYER = 14;
    public const int BACK_OF_CUBE_LAYER = 15;
    
    // Input
    public const string MOUSE_X_INPUT = "Mouse X";
    public const string MOUSE_Y_INPUT = "Mouse Y";
    public const string MOUSE_SCROLL_INPUT = "Mouse ScrollWheel";
    public const string FIRE_BUTTON_INPUT = "Fire1";
    
    // Variables
    public static float FRONT_OF_CUBE_ANGLE = 60.0f;
    public static float SIDE_OF_CUBE_ANGLE = 120.0f;
    public static float ROTATE_SPEED = 0.5f;
    public static float SHUFFLE_SPEED = 0.1f;
    public static float CAMERA_ROTATE_SPEED = 100f;
    public static float CAMERA_ROTATE_TIME = 5.0f;

    public static int X_TRIGGER_AXIS = 1;
    public static int Y_TRIGGER_AXIS = 2;
    public static int Z_TRIGGER_AXIS = 3;
    public static int MAX_SHUFFLES_SMALL = 25;
    public static int MAX_SHULLES_LARGE = 50;
}