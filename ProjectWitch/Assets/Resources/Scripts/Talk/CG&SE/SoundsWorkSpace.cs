//=====================================
//author	:shotta
//summary	:サウンドの作業場
//=====================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System; //Exception
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

using Scenario.Pattern;
using Scenario.Command;

namespace Scenario.WorkSpace
{
	public class SoundsWorkSpace : MonoBehaviour
	{
		//BGM再生(ヘルパー関数)
		public void PlayBGM(string fileName, int volume)
		{
			mBGMSource1.GetComponent<AnimationController> ().SetAnimation (null);
			mBGMSource2.GetComponent<AnimationController> ().SetAnimation (null);
			float volume1 = mBGMSource1.GetComponent<AudioSource> ().volume;
			float volume2 = mBGMSource2.GetComponent<AudioSource> ().volume;

			AudioSource audioSource = null;
			AnimationController fadeoutAudio = null;

			if (volume1 < volume2) {
				audioSource = mBGMSource1.GetComponent<AudioSource> ();
				fadeoutAudio = mBGMSource2.GetComponent<AnimationController> ();
			} else {
				audioSource = mBGMSource2.GetComponent<AudioSource> ();
				fadeoutAudio = mBGMSource1.GetComponent<AnimationController> ();
			}

			if (fileName != null) {
				audioSource.clip = (AudioClip)Resources.Load (mBGMPath + fileName);
				audioSource.Play ();
				audioSource.volume = (float)volume / 100.0f;
			}

			fadeoutAudio.SetAnimation (new FadeOutBGM ());
		}

		//BGMのパス
		[SerializeField]
		private string mBGMPath;

		//BGMの発生源
		[SerializeField]
		private GameObject mBGMSource1;
		[SerializeField]
		private GameObject mBGMSource2;

		public class FadeOutBGM : AnimationFormat
		{
			private AudioSource mAudio;
			public override void Setup (GameObject target)
			{
				mAudio = target.GetComponent<AudioSource> ();
			}
			public override void Update (GameObject target)
			{
				AudioSource audio = mAudio;
				float volume = audio.volume;
				if (volume > 0.01) {
					audio.volume = volume * Mathf.Pow (0.25f, Time.deltaTime);
				} else {
					audio.volume = 0.0f;
					audio.Stop ();
					SetActive (false);
				}
			}
			public override void Finish (GameObject target)
			{
				mAudio = null;
			}
		}

		//SE再生
		public void PlaySE(string fileName, float volume)
		{
			AudioSource audio = mSESource;

			audio.clip = (AudioClip)Resources.Load (mSEPath + fileName);
			audio.volume = (float)volume/100.0f;
			audio.Play ();
		}

		//SEのパス
		[SerializeField]
		private string mSEPath;

		//SEの発生源
		[SerializeField]
		private AudioSource mSESource;

		//Voice再生
		public void PlayVoice(string fileName, float volume)
		{
			AudioSource audio = mVoiceSource;
			audio.clip = (AudioClip)Resources.Load (mVoicePath + fileName);
			audio.volume = (float)volume/100.0f;
			audio.Play ();
		}
		//Voice停止
		public void StopVoice()
		{
			AudioSource audio = mVoiceSource;
			audio.Stop ();
		}

		//Voiceのパス
		[SerializeField]
		private string mVoicePath;

		//Voiceの発生源
		[SerializeField]
		private AudioSource mVoiceSource;

		//SE&Voiceが止まっているかどうかを調べる
		public bool IsPlayingSEAndVoice()
		{
			bool isPlaying = false;
			isPlaying |= mSESource.isPlaying;
			isPlaying |= mVoiceSource.isPlaying;
			return isPlaying;
		}

		public void SetCommandDelegaters(VirtualMachine vm)
		{
			//BGMを再生
			vm.AddCommandDelegater (
				"PlayBGM",
				new CommandDelegater(false, 2, delegate(object[] arguments){
					string error;
					string name = Converter.ObjectToString(arguments[0], out error);
					if (error != null) return error;
					int volume = Converter.ObjectToInt(arguments[1], out error);
					if (error != null) return error;

					PlayBGM(name, volume);
					return null;
				}));
			//BGMを停止
			vm.AddCommandDelegater (
				"StopBGM",
				new CommandDelegater(false, 0, delegate(object[] arguments){
					PlayBGM (null, 0);
					return null;
				}));
			//SEを再生
			vm.AddCommandDelegater (
				"PlaySE",
				new CommandDelegater(false, 2, delegate(object[] arguments){
					string error;
					string name = Converter.ObjectToString(arguments[0], out error);
					if (error != null) return error;
					int volume = Converter.ObjectToInt(arguments[1], out error);
					if (error != null) return error;

					PlaySE(name, volume);
					return null;
				}));

			//SEを再生
			vm.AddCommandDelegater (
				"PlayVoice",
				new CommandDelegater(false, 2, delegate(object[] arguments){
					string error;
					string name = Converter.ObjectToString(arguments[0], out error);
					if (error != null) return error;
					int volume = Converter.ObjectToInt(arguments[1], out error);
					if (error != null) return error; 

					PlayVoice(name, volume);
					return null;
				}));
		}
	}
}