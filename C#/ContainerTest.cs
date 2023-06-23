using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Debug = UnityEngine.Debug;

enum TestCase
{
    HashSetGameVer,
    List,
    Dictionary,
    dictionaryWithoutDuplicated,
}

public class TestData
{
    public string sku;
    public string name;
}

public class TestScript
{
    public class DataContainer
    {
        public List<TestData> datas = new List<TestData>();
    }

    private DataContainer dataContainer;
    private List<TestData> list;
    private HashSet<TestData> hashSetGameVer;
    private Dictionary<string, string> dictionary;
    private Dictionary<string, string> dictionaryWithoutDuplicated;
    private Stopwatch watch;

    private Dictionary<TestCase, long> times;
    // A Test behaves as an ordinary method

    public void SetTestData(int count = 5000)
    {
        dataContainer = new DataContainer();

        var generator = new RandomGenerator();
        for (int n = 0; n < count; n++)
        {
            dataContainer.datas.Add(new TestData() { sku = generator.RandomString(10), name = string.Empty });
        }

        int dataCount = dataContainer.datas.Count;
        for (int i = 0; i < dataCount; i++)
        {
            dataContainer.datas.Add(new TestData() { sku = dataContainer.datas[i].sku, name = string.Empty });
        }
    }

    public void AddData()
    {
        watch.Restart();

        for (int n = 0; n < dataContainer.datas.Count; n++)
        {
            bool isAdded = false;
            foreach (var data in hashSetGameVer)
            {
                if (data.sku == dataContainer.datas[n].sku)
                {
                    isAdded = true;
                    break;
                }
            }

            if (isAdded) continue;
            hashSetGameVer.Add(dataContainer.datas[n]);
        }

        watch.Stop();
        times[TestCase.HashSetGameVer] = watch.ElapsedMilliseconds;

        watch.Restart();

        for (int n = 0; n < dataContainer.datas.Count; n++)
        {
            bool isAdded = false;
            foreach (var data in list)
            {
                if (data.sku == dataContainer.datas[n].sku)
                {
                    isAdded = true;
                    break;
                }
            }
            if (isAdded) continue;
            list.Add(dataContainer.datas[n]);
        }

        watch.Stop();

        times[TestCase.List] = watch.ElapsedMilliseconds;

        watch.Restart();
        for (int n = 0; n < dataContainer.datas.Count; n++)
        {
            dictionary[dataContainer.datas[n].sku] = dataContainer.datas[n].name;
        }

        watch.Stop();

        times[TestCase.Dictionary] = watch.ElapsedMilliseconds;

        watch.Restart();
        for (int n = 0; n < dataContainer.datas.Count; n++)
        {
            if(dictionaryWithoutDuplicated.ContainsKey(dataContainer.datas[n].sku)) continue;
            dictionaryWithoutDuplicated[dataContainer.datas[n].sku] = dataContainer.datas[n].name;
        }

        watch.Stop();

        times[TestCase.dictionaryWithoutDuplicated] = watch.ElapsedMilliseconds;

        watch.Reset();

        Debug.Log("Add Data");

        // Debug.Log("hashSet.Count : " + hashSet.Count);
        Debug.Log("hashSetGameVer.Count : " + hashSetGameVer.Count);
        Debug.Log("list.Count : " + list.Count);
        Debug.Log("dictionary.Count : " + dictionary.Count);
        Debug.Log("dictionaryWithoutDuplicated.Count : " + dictionary.Count);
        Debug.Log("\n");
    }

    public void GetFastContainer()
    {
        var orderTimes = times.OrderBy(x => x.Value);

        int n = 1;
        foreach (var time in orderTimes)
        {
            Debug.Log(String.Format("{0}. {1} : {2} ms", n++, time.Key, time.Value));
        }

        Debug.Log("\n");
    }

    [Test]
    public void SKUContainerTest()
    {
        SetTestData(200);

        Debug.Log("testData.datas.Count : " + dataContainer.datas.Count);
        Debug.Log("\n");

        hashSetGameVer = new HashSet<TestData>();
        list = new List<TestData>();
        dictionary = new Dictionary<string, string>();
        dictionaryWithoutDuplicated = new Dictionary<string, string>();
        watch = new Stopwatch();

        times = new Dictionary<TestCase, long>();

        AddData();
        GetFastContainer();
    }

    //reference : https://www.c-sharpcorner.com/article/generating-random-number-and-string-in-C-Sharp/
    public class RandomGenerator
    {
        private readonly System.Random _random = new System.Random();

        // Generates a random string with a given size.
        public string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.

            // char is a single Unicode character
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length = 26

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return builder.ToString();
        }
    }
}
