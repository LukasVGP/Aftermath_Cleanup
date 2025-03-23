using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameIntroScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject introPanel;
    [SerializeField] private TextMeshProUGUI introText;
    [SerializeField] private Button closeButton;

    [Header("Settings")]
    [SerializeField] private bool showOnStart = true;
    [SerializeField] private KeyCode closeKey = KeyCode.Escape;
    [SerializeField] private KeyCode openKey = KeyCode.I;

    [Header("Game Instructions")]
    [TextArea(10, 20)]
    [SerializeField]
    private string gameInstructions =
        "ZOMBIE FACTORY INSTRUCTIONS\n\n" +
        "Welcome to Zombie Factory! Work together to dispose of zombies and earn points.\n\n" +
        "RED PLAYER CONTROLS:\n" +
        "- Move: A/D keys\n" +
        "- Jump: SPACE\n" +
        "- Grab/Release: Q\n" +
        "- Activate Red Lever: E (when close to lever)\n\n" +
        "BLUE PLAYER CONTROLS:\n" +
        "- Move: Numpad 4/6\n" +
        "- Jump: Right Shift\n" +
        "- Grab/Release: Numpad 7\n" +
        "- Activate Blue Lever: Numpad 9 (when close to lever)\n\n" +
        "GAMEPLAY:\n" +
        "- Grab zombies together to tear them apart\n" +
        "- Use the Red Lever to open the furnace lid\n" +
        "- Use the Blue Lever to activate the conveyor belt\n" +
        "- Dispose of zombies in the furnace for points\n" +
        "- Complete all waves to unlock the exit\n" +
        "- Both players must reach the exit to complete the level\n\n" +
        "Press ESC to close this screen. Press I to reopen it anytime.";

    private void Start()
    {
        if (introText != null)
        {
            introText.text = gameInstructions;
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseIntroScreen);
        }

        if (showOnStart)
        {
            ShowIntroScreen();
        }
        else
        {
            HideIntroScreen();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(closeKey) && introPanel.activeSelf)
        {
            CloseIntroScreen();
        }
        else if (Input.GetKeyDown(openKey) && !introPanel.activeSelf)
        {
            ShowIntroScreen();
        }
    }

    public void ShowIntroScreen()
    {
        introPanel.SetActive(true);

        // Pause the game while showing instructions
        Time.timeScale = 0f;
    }

    public void CloseIntroScreen()
    {
        introPanel.SetActive(false);

        // Resume the game
        Time.timeScale = 1f;
    }

    // Add the missing HideIntroScreen method
    public void HideIntroScreen()
    {
        // This is just an alias for CloseIntroScreen for consistency
        CloseIntroScreen();
    }

    public void ToggleIntroScreen()
    {
        if (introPanel.activeSelf)
        {
            CloseIntroScreen();
        }
        else
        {
            ShowIntroScreen();
        }
    }
}
