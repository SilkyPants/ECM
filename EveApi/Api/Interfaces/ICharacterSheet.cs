using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveApi.Api.Interfaces
{
    public interface ICharacterSheet
    {
        long ID
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }

        DateTime Birthday
        {
            get;
            set;
        }

        string Race
        {
            get;
            set;
        }

        string Bloodline
        {
            get;
            set;
        }

        string Ancestry
        {
            get;
            set;
        }

        string Gender
        {
            get;
            set;
        }

        string Corporation
        {
            get;
            set;
        }

        long CorporationID
        {
            get;
            set;
        }

        string Alliance
        {
            get;
            set;
        }

        long AllianceID
        {
            get;
            set;
        }

        string CloneName
        {
            get;
            set;
        }

        long CloneSkillPoints
        {
            get;
            set;
        }

        decimal AccountBalance
        {
            get;
            set;
        }

        ImplantSet Implants
        {
            get;
            set;
        }

        CharacterAttributes Attributes
        {
            get;
            set;
        }

        List<CharacterSkills> Skills
        {
            get;
            set;
        }

        List<CharacterCertificates> Certificates
        {
            get;
            set;
        }
    }
}
