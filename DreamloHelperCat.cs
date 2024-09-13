using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System.Linq;
//  _._    CODE BY   _._   //
//       /| ________________
// O|===|* >________________>
//       \|
//    _._     _,-'""`-._         _
//   (,-.`._,'(       |\`-/| ___/___
//       `-.-' \ )-`( , o o) |_____|
//             `-    \`_`"'-  | - |
//                            | - |
//  MILKSHAKE BATTLECAT 2024   ``` 
/// <summary>
/// Search "Change this" to find all various parts of this script that need your project specific edits.
/// SendHighScore() is the function you'll want to assign to a button to send your score.
/// </summary>
public class DreamloHelperCat : MonoBehaviour
{

    // NOTE: This script can only function if your dreamloLeaderBoard is in the scene
    private dreamloLeaderBoard dl;

    [Tooltip("Update the UI periodically in case new scores come in. Call UpdateLeaderboardUI() when you want to reload the scores list.")]
    public bool updateLeaderboardPeriodically = false;    
    [Tooltip("If updateLeaderboardPeriodically is true, score UI will be updated according to this float")]
    public float periodicUpdateInterval = 30f;

    [Space(20)]
    [Header("Amount of namesTexts and scoresTexts should be the same and should be ordered the same (namesText[0] matches scoresTexts[0] and so on). Assign as many as you want to display.")]    
    public List<TextMeshProUGUI> namesTexts;
    public List<TextMeshProUGUI> scoresTexts;
    [Space(20)]

    [Tooltip("Optional: Reports a response for your user after attempting to send a score")]
    public TextMeshProUGUI submissionResponseText;
    
    [Tooltip("Optional: Use this InputField to get your player name")]
    public TMP_InputField playerNameInputField;

    [Tooltip("Optional: Change to your preferred default player name")]
    public string defaultPlayerName = "Player";

    [Header("Don't forget to update the variables and logic in the script to suit your project's needs!")]

    // Change this to suit your needs!
    // Get this URL from your dreamlo site (Under "Get your data as XML")
    private const string leaderboardXMLUrl = "Your XML URL Goes Here";

    // Remember to change this to suit your needs! It is under "Delete Carmine's score" on your dreamlo page (remove 'Carmine' from end of URL, keep '/' at the end)
    private const string deletionURL = "Your deletion URL goes here";

    // Uses PlayerPrefs to store key. See GenerateAndSavePlayerID to change to your preferred save data method
    private const string PlayerIDKey = "PlayerID";

    private void OnDestroy()
    {
        // Remove listeners when the script is destroyed
        if(playerNameInputField != null)
        {
            playerNameInputField.onValueChanged.RemoveListener(ValidateInput);
        }        
    }    

    // Start is called before the first frame update
    void Start()
    {
        // Get reference to the dreamloLeaderBoard. This script can only function if your dreamloLeaderBoard is in the scene
        dl = dreamloLeaderBoard.GetSceneDreamloLeaderboard();

        // Generate unique player ID
        GenerateAndSavePlayerID();

        // Subscribe to the onValueChanged event
        if(playerNameInputField != null)
        {
            playerNameInputField.onValueChanged.AddListener(ValidateInput);
        }
        
    }

    private IEnumerator UpdateLeaderboardPeriodically()
    {
        while (updateLeaderboardPeriodically)
        {
            // Perform the leaderboard update
            UpdateLeaderboardUI();

            // Wait for 30 seconds before repeating
            yield return new WaitForSeconds(periodicUpdateInterval);
        }
    }

    public void UpdateLeaderboardUI()
    {
        // Optional: Handle loading stat here
        // namesTexts[0].text = "Loading...";

        StartCoroutine(UpdateLeaderboardTextsCoroutine());
    }

    private IEnumerator UpdateLeaderboardTextsCoroutine()
    {
        // Make the web request to get the XML data
        UnityWebRequest www = UnityWebRequest.Get(leaderboardXMLUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("Failed to fetch leaderboard data: " + www.error);
            yield break;
        }

        string responseText = www.downloadHandler.text;

        // Check for empty or invalid response
        if (string.IsNullOrEmpty(responseText))
        {
            Debug.LogWarning("Received empty response.");
            yield break;
        }

        XmlDocument xmlDoc = new XmlDocument();
        try
        {
            xmlDoc.LoadXml(responseText);
        }
        catch (XmlException ex)
        {
            Debug.LogWarning("Failed to parse XML data: " + ex.Message);
            yield break;
        }
        
        XmlNodeList entryList = xmlDoc.SelectNodes("//leaderboard/entry");
        Debug.Log($"Number of entries retrieved: {entryList.Count}");

        if (entryList == null || entryList.Count == 0)
        {
            Debug.LogWarning("No leaderboard data found. Nothing to display.");
            yield break;
        }

        // Iterate over the entryList and update the TextMeshProUGUI components
        int i = 0;
        foreach (XmlNode entryNode in entryList)
        {
            if (i < namesTexts.Count && i < scoresTexts.Count)
            {
                string playerName = entryNode.SelectSingleNode("name").InnerText;
                string score = entryNode.SelectSingleNode("score").InnerText;

                namesTexts[i].text = playerName;
                scoresTexts[i].text = score;

                i++;
            }
            else
            {
                // If we've run out of UI elements to update, break out of the loop
                break;
            }
        }

        // If there are more UI elements than entries, clear the remaining UI elements
        for (; i < namesTexts.Count; i++)
        {
            namesTexts[i].text = "";
            scoresTexts[i].text = "";
        }
    }

    // Change this function to suit your needs
    // Assign it to the OnClick function of a button or some such for use
    public void SendHighScore()
    {
        string myName = "Player";

        // Get name from input field if available
        if(playerNameInputField != null)
        {
            myName = GetInputFieldText(playerNameInputField);
        }        

        // Default value; change to your preferred logic (ignore if not using time variable, but a value is needed to send to the dl.AddScore function)
        int myTime = 10;

        // Default value; change to your preferred logic (such as get from PlayerPrefs)
        int myScore = 100;

        // Placehodler ASDF should never be used as long as GenerateAndSavePlayerID is called in Start()
        string myID = PlayerPrefs.GetString(PlayerIDKey, "ASDF");

        Debug.Log("Sending high score...");
        StartCoroutine(CheckAndSendScore(myName, myScore, myTime, myID));

        // OPTIONAL: logic for detecting if no score exists.
        // Just have your non-score default value as -1, for example: PlayerPrefs.GetInt("HighScore", -1);
        // if(myScore != -1)
        // {
        //     Debug.Log("Sending high score...");
        //     StartCoroutine(CheckAndSendScore(myName, myScore, myTime, myID));
        // }
        // else
        // {
        //     Debug.Log("High score could not be found to be sent");
        //     ResponseText("You don't have any scores to submit.");
        // }
        
    }

    private IEnumerator CheckAndSendScore(string myName, int myScore, int myTime, string myID)
    {
        // Fetch XML data
        using (UnityWebRequest www = UnityWebRequest.Get(leaderboardXMLUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("Failed to fetch leaderboard data: " + www.error);
                yield break;
            }

            string responseText = www.downloadHandler.text;
            // Check for empty or invalid response
            if (string.IsNullOrEmpty(responseText))
            {
                Debug.LogWarning("Received empty response.");
                yield break;
            }

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(responseText);
            }
            catch (XmlException ex)
            {
                Debug.LogWarning("Failed to parse XML data: " + ex.Message);
                yield break;
            }

            XmlNodeList entries = xmlDoc.GetElementsByTagName("entry");

            bool entryExists = false;
            bool nameExists = false;
            string entryNameToDelete = null;
            // Change this existingScore value if your scores are sometimes negative. It should be a value below any possible score.
            // For example you could use: int.MinValue, the smallest value that an int can represent (-2,147,483,648)
            int existingScore = -1;   

            foreach (XmlNode entry in entries)
            {
                // Access the child nodes
                XmlNode nameNode = entry.SelectSingleNode("name");
                XmlNode scoreNode = entry.SelectSingleNode("score");
                XmlNode textNode = entry.SelectSingleNode("text"); // This should be the node containing the ID

                string existingName = nameNode.InnerText;
                int score = int.Parse(scoreNode?.InnerText ?? "0");
                string id = textNode?.InnerText;

                if (id == myID || existingName == myName)
                {
                    entryExists = true;
                    existingScore = score; // Store the existing score
                    entryNameToDelete = existingName; // Store name for potential deletion

                    if (existingName == myName && id != myID)
                    {
                        // Name matches, but ID doesn't. Will deny use of already used name.
                        nameExists = true;
                    }
                    
                    // Break out of loop since either the name or ID already exists.
                    break;
                }                
            }

            if (entryExists || nameExists)
            {
                if(nameExists)
                {
                    Debug.LogWarning("That name is already in use with a different player ID.");
                    ResponseText($"Player name already exists. Please choose a different name to submit your score.");

                    yield return new WaitForSeconds(1.5f);
                    UpdateLeaderboardUI();
                    yield break;
                }        

                if (myScore > existingScore)
                {
                    string encodedName = UnityWebRequest.EscapeURL(entryNameToDelete);
                    // Delete the existing entry with the matching ID
                    
                    string deleteUrl = deletionURL + encodedName;
                    using (UnityWebRequest deleteRequest = UnityWebRequest.Get(deleteUrl))
                    {
                        yield return deleteRequest.SendWebRequest();

                        if (deleteRequest.result == UnityWebRequest.Result.Success)
                        {                            Debug.Log("Old score entry deleted successfully.");
                            // Add new score
                            dl.AddScore(myName, myScore, myTime, myID);
                            ResponseText("Score updated!");

                        }

                        else
                        {
                            Debug.LogError("Failed to delete old score entry. URL was : " + deleteUrl);
                            Debug.LogError("Response: " + deleteRequest.downloadHandler.text);
                        }
                    }
                }
                else
                {
                    Debug.Log("Send Score: ID exists, but score is not higher. No update made.");
                    ResponseText("You've already submitted a score. You will have to achieve a higher score to submit again.");
                }
            }
            else
            {
                Debug.Log("Name or ID does not exist. Adding new high score...");
                ResponseText("Score added!");
                dl.AddScore(myName, myScore, myTime, myID);
            }
        }

        yield return new WaitForSeconds(1.1f);
        UpdateLeaderboardUI();
    }

    // Method to get the current text from the InputField
    public string GetInputFieldText(TMP_InputField inputField)
    {
        if (inputField != null)
        {
            return inputField.text;
        }
        else
        {
            Debug.LogWarning("InputField reference is not set. Returning default player name if one is set. If string is empty, will return 'Player'.");

            if(string.IsNullOrEmpty(defaultPlayerName))
            {
                return "Player";
            }

            return defaultPlayerName;
        }
    }

    private void ResponseText(string responseText)
    {
        if(submissionResponseText != null)
        {
            submissionResponseText.text = responseText;
        }        
    }

    private void ValidateInput(string input)
    {
        // Regular expression to allow only alphabets (no spaces or special characters)
        string validInput = Regex.Replace(input, @"[^a-zA-Z]", "");

        PlayerPrefs.SetString("PreferredName", validInput);
        defaultPlayerName = validInput;
        PlayerPrefs.Save();

        // Ensure the input is not empty
        if (string.IsNullOrEmpty(validInput))
        {
            validInput = defaultPlayerName;
        }

        // Update the text field if the input was altered
        if (input != validInput)
        {
            playerNameInputField.text = validInput;

            // Move the caret to the end of the text
            playerNameInputField.caretPosition = validInput.Length;
        }
    }

    private void GenerateAndSavePlayerID()
    {
        // Check if a Player ID already exists in PlayerPrefs
        if (!PlayerPrefs.HasKey(PlayerIDKey))
        {
            // Generate a unique Player ID
            string playerID = GenerateUniquePlayerID();

            // Save the Player ID to PlayerPrefs
            PlayerPrefs.SetString(PlayerIDKey, playerID);
            PlayerPrefs.Save(); // Ensure the data is saved immediately

            Debug.Log("Generated and saved new Player ID: " + playerID);
        }
        else
        {
            // Retrieve the existing Player ID
            string existingPlayerID = PlayerPrefs.GetString(PlayerIDKey);
            Debug.Log("Existing Player ID: " + existingPlayerID);
        }
    }

    private string GenerateUniquePlayerID()
    {
        // Generate a unique ID using GUID
        return System.Guid.NewGuid().ToString();
    }
	
} // end DreamloHelperCat
//                    ^           ^
//                   /  \______ /  \
//                  /  ^          ^ \
//                 |                 |
//                |       "    "     |
//             -- |  _.--._   _.--._  | --
//       (@_  ===(= .      '        . =) ===
//  _     ) \_____\  '. :   <->  :'.  /_______________________
// (_)@8@8{}<__________|\____Y____/|___________________________>
//        )_/  milk  \       -     /  battle
//       (@   shake    `----------`    cat
