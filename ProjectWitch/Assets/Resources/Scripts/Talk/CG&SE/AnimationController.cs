//=====================================
//author	:shotta
//summary	:演出の作業場
//=====================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System; //Exception
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

namespace Scenario
{
	//アニメーションのコントローラー
	public class AnimationController : MonoBehaviour
	{
		private AnimationFormat mAnimation;
		//アニメーションを登録
		public void SetAnimation(AnimationFormat animation)
		{
			if (mAnimation != null)
				mAnimation.Finish (this.gameObject);
			mAnimation = animation;
			if (mAnimation != null)
				mAnimation.Setup (this.gameObject);
		}

		void Update()
		{
			if (mAnimation != null)
			{
				mAnimation.Update (this.gameObject);
				if (!mAnimation.IsActive)
					SetAnimation (null);
			}
		}
	}
	//アニメーションオブジェクトのフォーマット
	abstract public class AnimationFormat
	{
		public bool IsActive{ get { return mIsActive; } }
		protected void SetActive(bool isActive)
		{
			mIsActive = isActive;
		}
		private bool mIsActive = true;

		//アニメーション用
		abstract public void Setup (GameObject target);
		abstract public void Update (GameObject target);
		abstract public void Finish (GameObject target);
	}
}

