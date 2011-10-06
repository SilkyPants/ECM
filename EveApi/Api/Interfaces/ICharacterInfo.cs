using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveApi.Api.Interfaces
{
    public interface ICharacterInfo
    {
        
		/*
		<?xml version="1.0" encoding="UTF-8"?>
		<eveapi version="2">
		  <currentTime>2011-09-07 06:33:51</currentTime>
		  <result>
		    <characterID>91145028</characterID>
		    <characterName>Nyai Maricadie</characterName>
		    <race>Gallente</race>
		    <bloodline>Gallente</bloodline>
		    <accountBalance>636101.10</accountBalance>
		    <skillPoints>787152</skillPoints>
		    <nextTrainingEnds>2011-09-06 10:58:04</nextTrainingEnds>
		    <shipName />
		    <shipTypeID>606</shipTypeID>
		    <shipTypeName>Velator</shipTypeName>
		    <corporationID>1000168</corporationID>
		    <corporation>Federal Navy Academy</corporation>
		    <corporationDate>2011-08-20 07:24:00</corporationDate>
		    <lastKnownLocation>Couster</lastKnownLocation>
		    <securityStatus>0.000999975000000042</securityStatus>
		    <rowset name="employmentHistory" key="recordID" columns="recordID,corporationID,startDate">
		      <row recordID="17681093" corporationID="1000168" startDate="2011-08-20 07:24:00" />
		    </rowset>
		  </result>
		  <cachedUntil>2011-09-07 07:30:51</cachedUntil>
		</eveapi> 
		 */

        /// <summary>
        /// Character's ID for the API
        /// </summary>
        long ID
        {
            get;
            set;
        }

        /// <summary>
        /// The character's name.
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// The character's race. 
        /// </summary>
        string Race
        {
            get;
            set;
        }

        /// <summary>
        /// The character's bloodline. 
        /// </summary>
        string Bloodline
        {
            get;
            set;
        }

        /// <summary>
        /// Amount of ISK the character has. 
        /// </summary>
        double AccountBalance
        {
            get;
            set;
        }

        /// <summary>
        /// How many skill points the characters has.
        /// </summary>
        int SkillPoints
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the ship the character is currently piloting.
        /// </summary>
        string ShipName
        {
            get;
            set;
        }

        /// <summary>
        /// The type of ship the character is currently piloting.
        /// </summary>
        long ShipTypeID
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the type of ship the character is currently piloting.
        /// </summary>
        string ShipTypeName
        {
            get;
            set;
        }

        /// <summary>
        /// The ID of the character's corporation.
        /// </summary>
        long CorporationID
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the character's corporation
        /// </summary>
        string Corporation
        {
            get;
            set;
        }

        /// <summary>
        /// The date the character joined the corporation. 
        /// </summary>
        DateTime CorporationDate
        {
            get;
            set;
        }

        /// <summary>
        /// The ID of the character's alliance. 
        /// </summary>
        long AllianceID
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the character's alliance. 
        /// </summary>
        string Alliance
        {
            get;
            set;
        }

        /// <summary>
        /// The date the character joined the alliance. 
        /// </summary>
        DateTime AllianceDate
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the character's current location.
        /// </summary>
        string LastKnownLocation
        {
            get;
            set;
        }

        /// <summary>
        /// The standing the character has with CONCORD.
        /// </summary>
        double SecurityStatus
        {
            get;
            set;
        }
    }
}
