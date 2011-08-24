using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveApi.Character
{
    public abstract class CharacterApiBase : ApiBase
    {
        /// <summary>
        /// ID of the character to pull the information for.
        /// </summary>
        public int CharacterID
        {
            get;
            set;
        }

        public override void AddDataToPost(PostSubmitter post)
        {
            base.AddDataToPost(post);

            post.PostItems.Add("characterID", CharacterID.ToString());
        }

    }
}
