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
			if (fileName != null) {
                var game = Game.GetInstance();
                game.SoundManager.Play(fileName, SoundType.BGM);
			}
		}

        public void StopBGM()
        {
            var game = Game.GetInstance();
            game.SoundManager.Stop(SoundType.BGM);
        }

		//SE再生
		public void PlaySE(string fileName, float volume)
		{
            var game = Game.GetInstance();

            game.SoundManager.Play(fileName, SoundType.SE);
		}

		//Voice再生
		public void PlayVoice(string fileName, float volume)
		{
            var game = Game.GetInstance();

            game.SoundManager.Play(fileName, SoundType.Voice);
		}
		//Voice停止
		public void StopVoice()
		{
            var game = Game.GetInstance();

            game.SoundManager.Stop(SoundType.Voice);
		}

		//SE&Voiceが再生しているかどうか
		public bool IsPlayingSEAndVoice()
		{
            var game = Game.GetInstance();

			bool isPlaying = false;
			isPlaying |= game.SoundManager.IsPlaying(SoundType.SE);
			isPlaying |= game.SoundManager.IsPlaying(SoundType.Voice);

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
					StopBGM ();
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