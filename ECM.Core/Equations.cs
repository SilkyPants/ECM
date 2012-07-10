using System;
using System.Collections.Generic;

namespace ECM.API.EVE
{
    /// <summary>
    /// Static class to help work out different Eve Equations
    /// </summary>
    public static class Equations
    {
		#region Skillpoints
		public static int SkillpointsAtLevel(int skillLevel, int skillRank)
		{
			return (int)Math.Pow(2, (2.5f*skillLevel)-2.5f) * 250 * skillRank;
		}
		
		public static float SkillpointsPerMinute(float effectivePrimary, float effectiveSecondary)
		{
			return effectivePrimary + (effectiveSecondary / 2);
		}
		#endregion
		
		#region Fighting
		public static float AlignTimeInSeconds(float inertiaModifier, float mass)
		{
			return (float)(Math.Log(2) * inertiaModifier * mass) / 500000;
		}
		#endregion
    }
}
