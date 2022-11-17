using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using UnityEngine.UI;
using TMPro;

public class LeaderBoardController : MonoBehaviour
{
   public TMP_InputField MemberName, PlayerTime;
   public TextMeshProUGUI playerNames, playerTimes;
   public int ID;
   public int scoreToUpload;
   public int scoreToBeat;
   
   public TMP_Text[] entries;
   public TMP_Text[] names;
   public TMP_Text conectText;
    public GameObject leaderboard;
    public GameObject inputInfo;
    public GameObject _reconnect;


void Start()
{
    StartCoroutine(SetupRoutine());
}

IEnumerator SetupRoutine()
{
    leaderboard.SetActive(false);
    inputInfo.SetActive(false);
    _reconnect.SetActive(false);
    conectText.text = "Connecting...";
    yield return LoginRoutine();
    yield return GetPlayerName();
    yield return HighSorcesFetchRoutine();
}
    IEnumerator GetPlayerName()
    {
        bool done = false;
          LootLockerSDKManager.GetPlayerName((response) =>
{
                    if (response.success)
                    {

                        Debug.Log("Successfully retrieved player name: " + response.name);

                        done = true;
                    } else
                    {
                        Debug.Log("Error getting player name");
                        done = true;
                    }
                });
                yield return new WaitWhile(() => done = false);
    }

    IEnumerator GetPlayerFiles()
    {
                bool done = false;
            LootLockerSDKManager.GetAllPlayerFiles((response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully retrieved player files: " + response.items.Length);
            } 
            else
            {
                Debug.Log("Error retrieving player storage");
            }
        });
        yield return new WaitWhile(() => done = false);
        }
        
    
   /*(void Awake()
   {
    Debug.Log("Start play");
    LootLockerSDKManager.StartSession("Player", (response) =>
    {
        Debug.Log("Starrted");
        if (response.success)
        {
            Debug.Log("Sent");
        }
        else
        {
            Debug.Log("Failed");
        }
    });
   }*/

   IEnumerator LoginRoutine()
   {
    bool done = false;
    LootLockerSDKManager.StartGuestSession((response) =>
    {
        Debug.Log("Starrted");
        if (response.success)
        {
            conectText.text = "";
            leaderboard.SetActive(true);
            Debug.Log("Player was logged in");
            PlayerPrefs.SetString("PlayerID", response.player_id.ToString());
            PlayerPrefs.SetString("PlayerName", response.player_identifier);
            Debug.Log(response.player_identifier);
            done = true;
        }
        else
        {
            conectText.text = "You're offline";
            _reconnect.SetActive(true);
            Debug.Log("Could not start session");
            done = true;
        }
    });
    yield return new WaitWhile(() => done == false);

   }

   public IEnumerator SubmitScoreRoutine()
   {
    bool done = false;
    string PlayerID = PlayerPrefs.GetString("PlayerID");
    LootLockerSDKManager.SubmitScore(PlayerID, int.Parse(PlayerTime.text), ID, (response) =>
    {
        if (response.success)
        {
            
            Debug.Log("Succesfully uploaded score");
            done = true;
        }
        else
        {
            Debug.Log("Failed" + response.Error);
            done = true;
        }
    });
        yield return new WaitWhile(() => done == false);


   }

        public IEnumerator HighSorcesFetchRoutine()
        {
            bool done = false;
            LootLockerSDKManager.GetScoreList(ID, 3, 0, (response) => 
            {
                if (response.success)
                {
                   // string tempPlayerNames = "Names\n";
                  //  string tempPlayerTimes = "Times\n";

                    LootLockerLeaderboardMember[] members = response.items;
                    
                    for (int i = 0; i < members.Length; i++)
                    {
                        if (i <= 3)
                        {
                            Debug.Log("i = 3");
                           scoreToBeat = members[i].score;
                        }
                        name = members[i].player.name;
                        Debug.Log(members[i].rank);
                        entries[i].text = (members[i].rank + ". " + "username: " + members[i].player.name + " Time: " + members[i].score);
                       string memberid = members[i].member_id;
                        /*else
                        {
                            Debug.Log("null");
                            names[i].text = (members[i].rank + ". " + members[i].player.id);
                        }*/
                    }
                        if (members.Length < 3)
                        {
                            for(int i = members.Length; i < 3; i++)
                            {
                                entries[i].text = (i + 1).ToString() + ".   none";
                            }
                            //tempPlayerNames += members[i].player.name;
                        }
                       /* else
                        {
                            tempPlayerNames += members[i].player.id;
                        }
                        tempPlayerTimes += members[i].score + "\n";
                        tempPlayerNames += "\n";
                        
                    }
                    done = true;
                    playerNames.text = tempPlayerNames;
                    playerTimes.text = tempPlayerTimes;
                }*/
                else 
                {
                    Debug.Log("Failed" + response.Error);
                    done = true;
                }
                  
                
            }});
            yield return new WaitWhile(() => done = false);
            
        }
        public void SetPlayerName()
        {
            LootLockerSDKManager.SetPlayerName(MemberName.text, (response) =>
            {
                if (response.success)
        {
            Debug.Log("Succesfully set player name");
            
        }
        else
        {
            Debug.Log("Could not set player name " + response.Error);
            
        }
            });
        }

        void Update()
        {
            if (scoreToUpload < scoreToBeat)
            {
                inputInfo.SetActive(true);
            }
        }
        
        IEnumerator SubmitTimeIEnumerator()
        {
            yield return SubmitScoreRoutine();
            yield return HighSorcesFetchRoutine();
        }

        public void SubmitTime()
        {
            StartCoroutine(SubmitTimeIEnumerator());
        }

        public void Reconnect()
        {
            StartCoroutine(SetupRoutine());
        }
}

