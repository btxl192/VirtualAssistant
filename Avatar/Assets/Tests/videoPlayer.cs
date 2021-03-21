using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class videoPlayer
    {
        [UnityTest]
        public IEnumerator videoPlayerWithEnumeratorPasses()
        {
            SceneManager.LoadScene("Scene1");
            yield return null;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Scene1"));
            yield return new WaitForSeconds(1);
            GameObject vp = GameObject.Find("Video Player");
            var vps = vp.GetComponent<videoPlayerScript>();
            var vpc = vp.GetComponent<UnityEngine.Video.VideoPlayer>();
            var anim = vp.GetComponent<Animator>();
            Assert.IsFalse(vpc.isPlaying);
            Assert.IsTrue(anim.GetBool("stopped"));
            yield return null;
            vps.HandleMsg(null, "VidUrl", "https://mw-public-data.s3.eu-west-2.amazonaws.com/613ad8d53cc7695cf01e74ebdb813d4879fb37e55290186e2175440841df21e2/nttdata-about.mp4");
            vps.HandleMsg(null, "VidControl", "Play");
            yield return new WaitForSeconds(1);
            Assert.IsTrue(vpc.isPlaying);
            Assert.IsFalse(vpc.isPaused);
            Assert.IsFalse(anim.GetBool("stopped"));
            vps.HandleMsg(null, "VidControl", "Pause");
            yield return null;
            Assert.IsTrue(vpc.isPaused);
            Assert.IsFalse(anim.GetBool("stopped"));
            vps.HandleMsg(null, "VidControl", "Resume");
            yield return null;
            Assert.IsFalse(vpc.isPaused);
            vps.HandleMsg(null, "VidControl", "Stop");
            yield return new WaitForSeconds(1);
            Assert.IsFalse(vpc.isPlaying);
            Assert.IsTrue(anim.GetBool("stopped"));
            yield return null;
        }
    }
}
