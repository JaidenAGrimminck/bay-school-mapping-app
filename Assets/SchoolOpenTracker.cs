using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class SchoolOpenTracker : MonoBehaviour
{
    public string breakLink = "https://raw.githubusercontent.com/FairfieldBW/clock/main/src/data/breaks.json";
    public string immersivesLink = "https://raw.githubusercontent.com/FairfieldBW/clock/main/src/data/immersives.json";
    public string scheduleLink = "https://raw.githubusercontent.com/FairfieldBW/clock/main/src/data/schedule.json";
    public string specialSchedule = "https://raw.githubusercontent.com/FairfieldBW/clock/main/src/data/special_schedule.json";

    public SettingsManager settingsManager;
    public TMPro.TextMeshProUGUI schoolInSessionText;
    public TMPro.TextMeshProUGUI currentClassText;

    // Start is called before the first frame update
    // void Start()
    // {
    //     StartCoroutine(updateData());
    //     StartCoroutine(updateDataBool());
    // }

    bool firstEnable = false;

    void OnEnable() {
        if (!firstEnable) { firstEnable = true; return;}
        StartCoroutine(updateData());
        StartCoroutine(updateDataBool());
    }

    bool restartCoroutine = false;

    // Update is called once per frame
    void Update()
    {
        if (restartCoroutine) {
            StartCoroutine(updateData());
            StartCoroutine(updateDataBool());
        }
    }

    IEnumerator updateDataBool() {
        yield return new WaitForSeconds(60.0f);
        restartCoroutine = true;
    }

    public IEnumerator updateData() {
        Debug.Log("checking if school is in session.");


        //Get and return the data from the url
        CoroutineData<UnityWebRequestAsyncOperation> request = new CoroutineData<UnityWebRequestAsyncOperation>(
            this, 
            GetRequest(breakLink)
        );

        Debug.Log("awaiting data from: " + breakLink);
        yield return request.coroutine;

        UnityWebRequestAsyncOperation result = request.result;

        if (result.webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + result.webRequest.error);
            yield break;
        }

        Debug.Log("break request was successful.");

        string json = result.webRequest.downloadHandler.text;

        //Get the current date
        DateTime dateTime = DateTime.Now;

        Debug.Log("current time: " + dateTime.ToString("D"));
        
        string[] dateFormatted = dateTime.ToString("D").Split(new string[] { ", " }, StringSplitOptions.None);

        string weekday = dateFormatted[0];
        string date = dateFormatted[1];
        string day = date.Split(' ')[1];
        string month = date.Split(' ')[0];
        string year = dateFormatted[2];

        if (weekday.ToLower() == "sunday" || weekday.ToLower() == "saturday") {
            //Is weekend, set school to not in session.
            schoolInSessionText.text = "No";
            currentClassText.text = "Enjoy your weekend!\nFinish your hw btw!";
            Debug.Log("is weekend, not in session right now.");
        } else {
            List<string> list = new List<string>();

            int start = json.IndexOf('[');
            int end = json.IndexOf(']');

            int count = 0;
            while (start != -1 && end != -1 && count < 25) {
                string addl = json.Substring(start, end - start + 1);

                json = json.Substring(end + 1);

                //Add to list
                list.Add(addl);

                //Get the next start and end
                start = json.IndexOf('[');
                end = json.IndexOf(']');

                //If there is no end, break
                if (end == -1 || start == -1) break;

                count++;
            }

            bool isSchoolOn = true;

            foreach (string bdate in list) {
                string dates = bdate;
                if (bdate.Contains("],")) {
                    dates = bdate.Substring(0, bdate.IndexOf("],"));
                }

                dates = dates.Replace("[", "");
                dates = dates.Replace("]", "");

                string datesAdj = dates.Replace("\"", string.Empty);
                string[] splitDates = datesAdj.Split(new string[] { ", " }, StringSplitOptions.None);

                DateTime date1 = new DateTime(
                    Int32.Parse(string.Join(string.Empty, splitDates[0].Split('/')[0])), 
                    Int32.Parse(string.Join(string.Empty, splitDates[0].Split('/')[1])), 
                    Int32.Parse(string.Join(string.Empty, splitDates[0].Split('/')[2]))
                );

                DateTime date2 = new DateTime(
                    Int32.Parse(string.Join(string.Empty, splitDates[1].Split('/')[0])), 
                    Int32.Parse(string.Join(string.Empty, splitDates[1].Split('/')[1])), 
                    Int32.Parse(string.Join(string.Empty, splitDates[1].Split('/')[2]))
                );

                DateTime now = DateTime.Now;

                if (date1.Ticks < now.Ticks && now.Ticks < dateTime.Ticks) {
                    //No school right now.
                    isSchoolOn = false;
                    Debug.Log("inside a break");
                    break;
                }
            }

            if (isSchoolOn) {
                Debug.Log("not inside a break");
                //Check the times.
                StartCoroutine(checkAndUpdateClass());
            } else {
                Debug.Log("no school right now.");
                schoolInSessionText.text = "No";
                currentClassText.text = "None Right Now";
            }
        }
    }

    IEnumerator checkAndUpdateClass() {
        CoroutineData<UnityWebRequestAsyncOperation> request = new CoroutineData<UnityWebRequestAsyncOperation>(
            this, 
            GetRequest(scheduleLink)
        );

        Debug.Log("getting data from " + scheduleLink);

        yield return request.coroutine;

        UnityWebRequestAsyncOperation result = request.result;

        if (result.webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + result.webRequest.error);
            yield break;
        }

        string json = result.webRequest.downloadHandler.text;

        string scheduleJson = json;

        Debug.Log("got data from " + scheduleLink);

        request = new CoroutineData<UnityWebRequestAsyncOperation>(
            this,
            GetRequest(specialSchedule)
        );

        Debug.Log("getting data from " + specialSchedule);

        yield return request.coroutine;

        result = request.result;

        if (result.webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + result.webRequest.error);
            yield break;
        }

        json = result.webRequest.downloadHandler.text;

        string specialScheduleJson = json;

        //Get the current date
        DateTime dateTime = DateTime.Now;

        //get weekday
        string[] dateFormatted = dateTime.ToString("D").Split(new string[] { ", " }, StringSplitOptions.None);

        string weekday = dateFormatted[0];

        string year = dateFormatted[2];

        string relativeDate = dateFormatted[1];
        string day = relativeDate.Split(' ')[1];
        string month = relativeDate.Split(' ')[0];

        string[] months = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        int monthNum = getIndexOf(months, month) + 1;

        Debug.Log(year + "/" + monthNum + "/" + day);

        specialScheduleJson = specialScheduleJson.Replace("/0", "/");

        string daySchedule = "";

        if (specialScheduleJson.Contains(year + "/" + monthNum + "/" + day)) {
            string[] split_schedule = specialScheduleJson.Split(new string[] { "\"" + year + "/" + monthNum + "/" + day + "\"" }, StringSplitOptions.None);

            string schedule = split_schedule[1].Substring(split_schedule[1].IndexOf(":") + 1, split_schedule[1].IndexOf("}") - split_schedule[1].IndexOf(":") - 1);

            daySchedule = schedule.Replace("{", "");
        } else {
            string[] split_schedule = scheduleJson.Split(new string[] { weekday + "\"" }, StringSplitOptions.None);

            string schedule = split_schedule[1].Substring(split_schedule[1].IndexOf(":") + 1, split_schedule[1].IndexOf("}") - split_schedule[1].IndexOf(":") - 1);

            daySchedule = schedule.Replace("{", "");
        }

        if (daySchedule.EndsWith("]")) daySchedule = daySchedule + ",";
        
        Dictionary<string, Period> scheduleHash = new Dictionary<string, Period>();

        int count = 0;

        //JSON Schema:
        /*
        "<name>": [
            [<hour>, <min>],
            [<hour>, <min>]
        ]
        */

        daySchedule = daySchedule.Replace("Activities + Sports/Drama", "ZZZ");

        string dayScheduleCopy = daySchedule + "";

        //ZZZ to make sure it isn't replaced with a period

        int start = dayScheduleCopy.IndexOf('\"');
        int end = dayScheduleCopy.IndexOf('\"', start + 1);

        while (start != -1 && end != -1 && count < 24) {
            string name_ = dayScheduleCopy.Substring(start + 1, end - start - 1);

            //Replace the name in that place with "timings"
            dayScheduleCopy = dayScheduleCopy.Replace(name_, "timings");

            int indexOfSecondEndBracket = dayScheduleCopy.IndexOf(
                "],", 
                dayScheduleCopy.IndexOf("],") + 1
            ); 

            string timings;

            bool lastObj = false;

            if (indexOfSecondEndBracket == -1) {
                indexOfSecondEndBracket = dayScheduleCopy.LastIndexOf("]");

                timings = dayScheduleCopy.Substring(
                    start,
                    indexOfSecondEndBracket - start + 1
                );

                lastObj = true;
            } else {
                //Extract the string between start and indexOfSecond end bracket including the brackets and the comma
                timings = dayScheduleCopy.Substring(
                    start, 
                    indexOfSecondEndBracket + 1
                );
            }            

            timings = timings.Replace("\n", "");
            //Replace all tabs
            timings = timings.Replace("\t", "");

            if (timings.EndsWith(",")) timings = timings.Substring(0, timings.Length - 1);

            //Parse timings as JSON
            timings = "{ " + timings + " }";
            
            Period period = parsePeriodJSON(timings);

            if (name_ == "ZZZ") {
                name_ = "Activities + Sports/Drama";
            }

            //Add the period to the hash
            scheduleHash.Add(name_, period);

            if (lastObj) break;

            //Remove the period from the string
            dayScheduleCopy = dayScheduleCopy.Remove(start, indexOfSecondEndBracket + 1);
            
            //Find the next start bracket
            start = dayScheduleCopy.IndexOf('\"');
            //Find the next end bracket
            end = dayScheduleCopy.IndexOf('\"', start + 1);

            count++;
        }

        //Get the current time
        string[] timeFormatted = dateTime.ToString("t").Split(new string[] { ":" }, StringSplitOptions.None);
         
        string hour = timeFormatted[0];
        string minute = timeFormatted[1];

        Debug.Log(" it is currently " + hour + ":" + minute);

        Period currentPeriod = null;
        DateTime first = new DateTime(), last = new DateTime();
        string currentName = "";

        int index = 0;

        //Loop through the times and log them
        foreach (KeyValuePair<string, Period> entry in scheduleHash) {
            string name = entry.Key;
            Period times = entry.Value;

            Debug.Log(times.timings[0][0] + ":" + times.timings[0][1] + " - " + times.timings[1][0] + ":" + times.timings[1][1] + " " + name);

            DateTime _start = new DateTime(
                int.Parse(year),
                monthNum,
                int.Parse(day),
                ((times.timings[0][1] - 10 < 0) ? -1 : 0) + times.timings[0][0],
                ((times.timings[0][1] - 10 < 0) ? 60 : 0) + times.timings[0][1] - 10, //10 minutes buffer
                0
            );

            DateTime end_ = new DateTime(
                int.Parse(year),
                monthNum,
                int.Parse(day),
                ((times.timings[1][1] - 10 < 0) ? -1 : 0) + times.timings[1][0],
                ((times.timings[1][1] - 10 < 0) ? 60 : 0) + times.timings[1][1] - 10,
                0
            );

            if (index == 0) first = _start;
            if (index == scheduleHash.Count - 1) last = end_;

            if (_start.Ticks < dateTime.Ticks && end_.Ticks > dateTime.Ticks) {
                currentPeriod = times;
                currentName = name;
            }

            index++;
        }

        if (last.Ticks < dateTime.Ticks) {
            currentClassText.text = "Classes have ended.";
            yield break;
        }

        if (dateTime.Ticks < first.Ticks) {
            currentClassText.text = "School hasn't started yet.";
            yield break;
        }

        if (currentPeriod == null) {
            Debug.Log("No period found"); //Fix this, in passing there is an error.
            currentClassText.text = "Error, No Block Found";
            yield break;
        }

        Debug.Log("Current Period is " + currentName);

        int blockIndex = settingsManager.getBlockIndex(currentName);

        string nameOfBlock = "";

        try {
            nameOfBlock = blockIndex > -1 ? settingsManager.getBlock(blockIndex) : currentName;
        } catch (Exception e) {
            Debug.Log("Error: " + e.Message);
            nameOfBlock = "err, " + currentName;
        }

        currentClassText.text = "Current Block: \n" + nameOfBlock;
        //Prob replace the block with the actual class name using settings.
        schoolInSessionText.text = "Yes";
    }

    IEnumerator GetRequest(string uri) {
        UnityWebRequest request = UnityWebRequest.Get(uri);

        yield return request.SendWebRequest();
    }

    private int getIndexOf(string[] str, string value) {
        for (int i = 0; i < str.Length; i++) {
            if (str[i] == value) {
                return i;
            }
        }

        return -1;
    }

    private Period parsePeriodJSON(string json) {
        string copy = json;
        //json format: { "timings": [[<hour>, <min>], [<hour>, <min>]] }
        copy = copy.Replace("}", "");
        copy = copy.Replace("{", "");
        copy = copy.Replace("[", "");
        copy = copy.Replace(" ", "");
        copy = copy.Replace("\n", "");
        copy = copy.Replace("\t", "");
        copy = copy.Replace("\"timings\":", "");
        copy = copy.Replace("]]", "");

        //new format: <hour>,<min>],<hour>,<min>]]
        string[] splitCopy = copy.Split(new string[] { "]," }, StringSplitOptions.None);

        Period newPeriod = new Period();

        newPeriod.timings = new int[][] {
            new int[] {
                int.Parse(splitCopy[0].Split(',')[0]),
                int.Parse(splitCopy[0].Split(',')[1])
            },
            new int[] {
                int.Parse(splitCopy[1].Split(',')[0]),
                int.Parse(splitCopy[1].Split(',')[1])
            }
        };

        return newPeriod;
    }

    [System.Serializable]
    class Period {
        public int[][] timings;
    }
}
