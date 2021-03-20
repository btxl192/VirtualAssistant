using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class videoPlayer
    {
        [UnityTest]
        public IEnumerator videoPlayerWithEnumeratorPasses()
        {
            GameObject vp = GameObject.Find("Video Player");
            var vpcom = vp.GetComponent<videoPlayerScript>();
            yield return null;
        }
    }
}
