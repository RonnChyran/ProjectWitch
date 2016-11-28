using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ProjectWitch.Battle
{
	class BattleRandom
	{
		static private readonly int[] iTable = { 1 };
		static private readonly float[] fTable = { 1.0f };
		static public bool IsFix { get; set; }
		static private int iPos = 0;
		static private int fPos = 0;
		static public int IPos
		{
			get { return iPos; }
			set
			{
				if (value >= 0 && value < iTable.Length)
					iPos = value;
			}
		}
		static public int FPos
		{
			get { return fPos; }
			set
			{
				if (value >= 0 && value < iTable.Length)
					fPos = value;
			}
		}

		static public void ResetPos()
		{
			IPos = 0;
			FPos = 0;
		}

		static public int Range(int min, int max)
		{
			if (IsFix)
			{
				if (IPos >= iTable.Length)
					IPos = 0;
				return System.Math.Max(System.Math.Min(iTable[IPos++], max), min);
			}
			else
			{
				return Random.Range(min, max);
			}
		}

		static public float value
		{
			get
			{
				if (IsFix)
				{
					if (FPos >= fTable.Length)
						FPos = 0;
					return fTable[FPos++];
				}
				else
				{
					return Random.value;
				}
			}
		}
	}

}
