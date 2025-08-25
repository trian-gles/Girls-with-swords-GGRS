using Godot;
using System.Collections.Generic;


public class ComboTrialManager : TutorialManager
{

	public override void AddChallenges()
	{
		switch (playerOne)
		{
			case 0:
				AddOLChallenges();
				return;
			case 1:
				AddGLChallenges();
				return;
			case 2:
                AddSLChallenges();
                return;
			case 3:
                AddHLChallenges();
                return;
		}

	}

	public override void _Ready()
	{
		comboTrial = true;
		base._Ready();
	}

	protected void AddOLChallenges()
	{
		Goal chargedHojoGoal = new Goal("Hojogiri, full charge", "special", "hold");
		// needs to be completed

		Goal dpGoal = new Goal("Dragon Punch", "right", "special")
		{
			p2StateFrame = 0,
			p1State = "AntiAir"
		};

		Goal hojogiriGoal = new Goal("Hojogiri", "special")
		{
			p2StateFrame = 0,
			p1State = "Hojogiri"
		};
		

		Goal sixSGoal = new Goal("Heavy slash", "right", "s")
		{
			p2StateFrame = 0,
			p1State = "6S"
		};

		Goal sixPGoal = new Goal("Uppercut", "right", "p")
		{
			p2StateFrame = 0,
			p1State = "6P"
		};



		Challenge basicComboChallenge = new Challenge("Easy Combo");
		basicComboChallenge.goals.Add(jabGoal);
		basicComboChallenge.goals.Add(kickGoal);
		basicComboChallenge.goals.Add(slashGoal);
		basicComboChallenge.goals.Add(cslashGoal);
		basicComboChallenge.goals.Add(hojogiriGoal);
		basicComboChallenge.MakeComboChallenge();
		challenges.Add(basicComboChallenge);

		Challenge basicAirCombo = new Challenge("Air Combo");
		basicAirCombo.goals.Add(sixPGoal);
		basicAirCombo.goals.Add(fJumpGoal);
		basicAirCombo.goals.Add(jKickGoal);
		basicAirCombo.goals.Add(jJabGoal);
		basicAirCombo.goals.Add(jKickGoal);
		basicAirCombo.goals.Add(dFJumpGoal);
		basicAirCombo.goals.Add(jKickGoal);
		basicAirCombo.goals.Add(jSlashGoal);
		basicAirCombo.goals.Add(dpGoal);
		basicAirCombo.MakeComboChallenge();
		challenges.Add(basicAirCombo);

        Challenge easyCornerThrowCombo = new Challenge("Corner throw combo", GameScene.ResetPos.P2CORNEREDRIGHT);
        easyCornerThrowCombo.goals.Add(grabGoal);
        easyCornerThrowCombo.goals.Add(kickGoal);
        easyCornerThrowCombo.goals.Add(hojogiriGoal);
        easyCornerThrowCombo.MakeComboChallenge();

        challenges.Add(easyCornerThrowCombo);

        Challenge midScreenPunish = new Challenge("Midscreen confirm/punish");
        midScreenPunish.goals.Add(cjabGoal);
        midScreenPunish.goals.Add(sixPGoal);
        midScreenPunish.goals.Add(fJumpGoal);
        midScreenPunish.goals.Add(jKickGoal);
        midScreenPunish.goals.Add(dpGoal);
        midScreenPunish.goals.Add(hojogiriGoal);
        midScreenPunish.MakeComboChallenge();

        challenges.Add(midScreenPunish);


        Challenge cornerThrowCombo = new Challenge("Hard corner throw", GameScene.ResetPos.P2CORNEREDRIGHT);
		cornerThrowCombo.goals.Add(grabGoal);
		cornerThrowCombo.goals.Add(sixSGoal);
		cornerThrowCombo.goals.Add(dpGoal);
		cornerThrowCombo.goals.Add(sixSGoal);
		cornerThrowCombo.goals.Add(hojogiriGoal);
		cornerThrowCombo.goals.Add(cjabGoal);
		cornerThrowCombo.goals.Add(kickGoal);
		cornerThrowCombo.goals.Add(sixSGoal);
		cornerThrowCombo.goals.Add(hojogiriGoal);
		cornerThrowCombo.MakeComboChallenge();

		challenges.Add(cornerThrowCombo);

		Challenge cornerPunish = new Challenge("Corner confirm/punish", GameScene.ResetPos.P2CORNEREDRIGHT);
		cornerPunish.goals.Add(cjabGoal);
		cornerPunish.goals.Add(sixPGoal);
		cornerPunish.goals.Add(fJumpGoal);
		cornerPunish.goals.Add(jKickGoal);
		cornerPunish.goals.Add(dpGoal);
		cornerPunish.goals.Add(sixSGoal);
		cornerPunish.goals.Add(hojogiriGoal);
		cornerPunish.goals.Add(cjabGoal);
		cornerPunish.goals.Add(kickGoal);
		cornerPunish.goals.Add(sixSGoal);
		cornerPunish.goals.Add(hojogiriGoal);
		cornerPunish.MakeComboChallenge();

		challenges.Add(cornerPunish);



	}

	protected void AddGLChallenges()
	{
		Challenge basicComboChallenge = new Challenge("Universal Combo");
		basicComboChallenge.goals.Add(cjabGoal);
		basicComboChallenge.goals.Add(kickGoal);
		basicComboChallenge.goals.Add(slashGoal);
		basicComboChallenge.goals.Add(cslashGoal);
        basicComboChallenge.MakeComboChallenge();
		challenges.Add(basicComboChallenge);

        Goal sixSGoal = new Goal("Heavy slash", "right", "s")
		{
			p2StateFrame = 0,
			p1State = "6C"
		};

		Goal sixPGoal = new Goal("Upper Kick", "right", "p")
		{
			p2StateFrame = 0,
			p1State = "3K"
		};

		Goal j2CGoal = new Goal("Downward Aerial Slash", "air", "down", "s")
		{
			p2StateFrame = 0,
			p1State = "J2C"
		};

		Goal gunBlazedGoal = new Goal("Gunblazed", "down", "special")
		{
			p2StateFrame = 0,
			p1State = "GunBlazed"
		};

		Goal superGoal = new Goal("OH SHIT", "right", "s", "special")
		{
			p2StateFrame = 0,
			p1State = "GLDP"
		};

        Goal dashAttackGoal = new Goal("Dash Attack", "right", "s")
        {
            p2StateFrame = 0,
            p1State = "DashAttack"
        };

        Challenge meterExtendedComboChallenge = new Challenge("Midscreen Metered Combo");
		meterExtendedComboChallenge.goals.Add(ckickGoal);
		meterExtendedComboChallenge.goals.Add(slashGoal);
		meterExtendedComboChallenge.goals.Add(sixSGoal);
		meterExtendedComboChallenge.goals.Add(fJumpGoal);
		meterExtendedComboChallenge.goals.Add(adGoal);
		meterExtendedComboChallenge.goals.Add(jSlashGoal);
		meterExtendedComboChallenge.goals.Add(jabGoal);
		meterExtendedComboChallenge.goals.Add(superGoal);
		meterExtendedComboChallenge.MakeComboChallenge();

        challenges.Add(meterExtendedComboChallenge);

        Challenge cornerComboChallenge = new Challenge("Big metered corner combo", GameScene.ResetPos.P2CORNEREDRIGHT);
        cornerComboChallenge.goals.Add(cslashGoal);
        cornerComboChallenge.goals.Add(gunBlazedGoal);
        Goal runGoal = new Goal("Run", "right", "dash");
        cornerComboChallenge.goals.Add(runGoal);
        cornerComboChallenge.goals.Add(dashAttackGoal);
        cornerComboChallenge.goals.Add(kickGoal);
        cornerComboChallenge.goals.Add(sixSGoal);
        cornerComboChallenge.goals.Add(fJumpGoal);
        cornerComboChallenge.goals.Add(adGoal);
        cornerComboChallenge.goals.Add(jSlashGoal);
        cornerComboChallenge.goals.Add(superGoal);
        cornerComboChallenge.MakeComboChallenge();
        challenges.Add(cornerComboChallenge);

        Challenge extendedComboChallenge = new Challenge("Hard Corner Carry Combo");
		dFJumpGoal.p1StateFrame = 1;
		dFJumpGoal.p1Tags = new HashSet<string>() { "aerial" };
		dFJumpGoal.p1State = null;
		extendedComboChallenge.goals.Add(ckickGoal);
		extendedComboChallenge.goals.Add(slashGoal);
		extendedComboChallenge.goals.Add(sixSGoal);
		extendedComboChallenge.goals.Add(fJumpGoal);
		extendedComboChallenge.goals.Add(adGoal);
		extendedComboChallenge.goals.Add(jSlashGoal);
		extendedComboChallenge.goals.Add(sixPGoal);
		extendedComboChallenge.goals.Add(fJumpGoal);
		extendedComboChallenge.goals.Add(jKickGoal);
		extendedComboChallenge.goals.Add(j2CGoal);
		extendedComboChallenge.goals.Add(slashGoal);
		extendedComboChallenge.goals.Add(gunBlazedGoal);
		extendedComboChallenge.goals.Add(cslashGoal);
        extendedComboChallenge.MakeComboChallenge();
        challenges.Add(extendedComboChallenge);

        



    }

	protected void AddHLChallenges()
	{
        Goal sixPGoal = new Goal("Uppercut", "right", "p")
        {
            p2StateFrame = 0,
            p1State = "6P"
        };

        Goal j2sGoal = new Goal("Down Slash", "air", "down", "s")
        {
            p2StateFrame = 0,
            p1State = "J2C"
        };

        Goal jrGoal = new Goal("Wheeeeee", "air", "special")
        {
            p2StateFrame = 0,
            p1State = "JoeRogan"
        };

        Goal superGoal = new Goal("OH SHIT", "right", "s", "special")
        {
            p2StateFrame = 0,
            p1State = "Super"
        };

        Challenge basicComboChallenge = new Challenge("Universal Combo");
		basicComboChallenge.goals.Add(jabGoal);
		basicComboChallenge.goals.Add(kickGoal);
		basicComboChallenge.goals.Add(slashGoal);
		basicComboChallenge.goals.Add(cslashGoal);
		basicComboChallenge.MakeComboChallenge();
        challenges.Add(basicComboChallenge);

        Challenge airCombo = new Challenge("Air Combo");
        airCombo.goals.Add(sixPGoal);
        airCombo.goals.Add(fJumpGoal);
        airCombo.goals.Add(jJabGoal);
        airCombo.goals.Add(jKickGoal);
        airCombo.goals.Add(jJabGoal);
        airCombo.goals.Add(jKickGoal);
        airCombo.goals.Add(dFJumpGoal);
        airCombo.goals.Add(jKickGoal);
        airCombo.goals.Add(jSlashGoal);
        airCombo.goals.Add(jrGoal);
		airCombo.MakeComboChallenge();
        challenges.Add(airCombo);


        Challenge cornerCarry = new Challenge("Corner Carry Combo");
        cornerCarry.goals.Add(ckickGoal);
        cornerCarry.goals.Add(sixPGoal);
        cornerCarry.goals.Add(fJumpGoal);
        cornerCarry.goals.Add(adGoal);
        cornerCarry.goals.Add(j2sGoal);
        cornerCarry.goals.Add(sixPGoal);
        cornerCarry.goals.Add(jJabGoal);
        cornerCarry.goals.Add(jSlashGoal);
        cornerCarry.goals.Add(sixPGoal);
        cornerCarry.goals.Add(fJumpGoal);
        cornerCarry.goals.Add(jJabGoal);
        cornerCarry.goals.Add(jKickGoal);
        cornerCarry.goals.Add(jSlashGoal);
        cornerCarry.goals.Add(jrGoal);
		cornerCarry.MakeComboChallenge();
        challenges.Add(cornerCarry);

        Challenge cornerCarryKD = new Challenge("Corner Carry Combo into Knockdown");
        cornerCarryKD.goals.Add(ckickGoal);
        cornerCarryKD.goals.Add(sixPGoal);
        cornerCarryKD.goals.Add(fJumpGoal);
        cornerCarryKD.goals.Add(adGoal);
        cornerCarryKD.goals.Add(j2sGoal);
        cornerCarryKD.goals.Add(sixPGoal);
        cornerCarryKD.goals.Add(jJabGoal);
        cornerCarryKD.goals.Add(jSlashGoal);
        cornerCarryKD.goals.Add(slashGoal);
        cornerCarryKD.goals.Add(superGoal);
		cornerCarryKD.MakeComboChallenge();
        challenges.Add(cornerCarryKD);
    }

	protected void AddSLChallenges()
	{
		Goal sixPGoal = new Goal("Uppercut", "right", "p")
		{
			p2StateFrame = 0,
			p1State = "6P"
		};

        Goal sixCGoal = new Goal("Heavy Slash (1 hit)", "right", "s")
        {
            p2StateFrame = 0,
            p1State = "6C"
        };

        Goal phoneTossGoal = new Goal("It's for you", "down", "special")
        {
            p2StateFrame = 0,
            p1State = "PhoneToss"
        };

        Goal j2CGoal = new Goal("Downward Aerial Slash", "air", "down", "s")
		{
			p2StateFrame = 0,
			p1State = "J2C"
		};

        Goal superGoal = new Goal("OH SHIT", "right", "s", "special")
        {
            p2StateFrame = 0,
            p1State = "SnailStrike"
        };


        Challenge basicComboChallenge = new Challenge("Universal Combo");
		basicComboChallenge.goals.Add(jabGoal);
		basicComboChallenge.goals.Add(kickGoal);
		basicComboChallenge.goals.Add(slashGoal);
		basicComboChallenge.goals.Add(cslashGoal);
        basicComboChallenge.MakeComboChallenge();
        challenges.Add(basicComboChallenge);

		Challenge airConfirm = new Challenge("Air combo into knockdown");
		airConfirm.goals.Add(sixPGoal);
		airConfirm.goals.Add(jKickGoal);
		airConfirm.goals.Add(jJabGoal);
		airConfirm.goals.Add(jKickGoal);
		airConfirm.goals.Add(j2CGoal);
		airConfirm.MakeComboChallenge();
		challenges.Add(airConfirm);

        Challenge bigDamage = new Challenge("Point blank big damage confirm");
        bigDamage.goals.Add(sixCGoal);
        bigDamage.goals.Add(phoneTossGoal);
        bigDamage.goals.Add(slashGoal);
        bigDamage.goals.Add(superGoal);
		bigDamage.MakeComboChallenge();
        challenges.Add(bigDamage);

        Challenge bigCornerDamage = new Challenge("Point blank corner big damage confirm", GameScene.ResetPos.P2CORNEREDRIGHT);
        bigCornerDamage.goals.Add(sixCGoal);
        bigCornerDamage.goals.Add(phoneTossGoal);
        bigCornerDamage.goals.Add(sixCGoal);
        bigCornerDamage.goals.Add(slashGoal);
        bigCornerDamage.goals.Add(superGoal);
        bigCornerDamage.MakeComboChallenge();
        challenges.Add(bigCornerDamage);
    }
}
