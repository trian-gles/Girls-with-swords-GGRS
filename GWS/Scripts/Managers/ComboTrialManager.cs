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
		RecordingName = "OL_combos";
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

		Goal sixKGoal = new Goal("Forward kick", "right", "k")
		{
			p2StateFrame = 0,
			p1State = "6K"
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

		Challenge midScreenPunish = new Challenge("Corner carry combo");
		midScreenPunish.goals.Add(ckickGoal);
		midScreenPunish.goals.Add(sixPGoal);
		midScreenPunish.goals.Add(sixSGoal);
		midScreenPunish.goals.Add(hojogiriGoal);
		midScreenPunish.goals.Add(cjabGoal);
		midScreenPunish.goals.Add(kickGoal);
		midScreenPunish.goals.Add(sixSGoal);
		midScreenPunish.goals.Add(hojogiriGoal);
		midScreenPunish.MakeComboChallenge();

		challenges.Add(midScreenPunish);


		Challenge cornerThrowCombo = new Challenge("Corner throw combo", GameScene.ResetPos.P2CORNEREDRIGHT);
		cornerThrowCombo.goals.Add(grabGoal);
		cornerThrowCombo.goals.Add(sixKGoal);
		cornerThrowCombo.goals.Add(dpGoal);
		cornerThrowCombo.goals.Add(sixPGoal);
		cornerThrowCombo.goals.Add(fJumpGoal);
		cornerThrowCombo.goals.Add(adGoal);
		cornerThrowCombo.goals.Add(jJabGoal);
		cornerThrowCombo.goals.Add(dpGoal);
		cornerThrowCombo.goals.Add(kickGoal);
		cornerThrowCombo.goals.Add(sixSGoal);
		cornerThrowCombo.goals.Add(hojogiriGoal);
		cornerThrowCombo.MakeComboChallenge();

		challenges.Add(cornerThrowCombo);

		Challenge cornerPunish = new Challenge("Corner punish", GameScene.ResetPos.P2CORNEREDRIGHT);
		cornerPunish.goals.Add(cjabGoal);
		cornerPunish.goals.Add(sixPGoal);
		cornerPunish.goals.Add(fJumpGoal);
		cornerPunish.goals.Add(jKickGoal);
		cornerPunish.goals.Add(jSlashGoal);
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
		RecordingName = "GL_combos";

		Challenge basicComboChallenge = new Challenge("Easy Combo");
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

		Challenge extendedComboChallenge = new Challenge("Hard Meterless Corner Carry Combo");
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
		RecordingName = "HL_combos";

		Goal sixPGoal = new Goal("Uppercut", "right", "p")
		{
			p2StateFrame = 0,
			p1State = "6P"
		};

		Goal sixCGoal = new Goal("Heavy Slash", "right", "s")
		{
			p2StateFrame = 0,
			p1State = "6S"
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

		Challenge basicComboChallenge = new Challenge("Easy Combo");
		basicComboChallenge.goals.Add(jabGoal);
		basicComboChallenge.goals.Add(kickGoal);
		basicComboChallenge.goals.Add(slashGoal);
		basicComboChallenge.goals.Add(cslashGoal);
		basicComboChallenge.MakeComboChallenge();
		challenges.Add(basicComboChallenge);

		Challenge airCombo = new Challenge("Easy Air Combo");
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
		cornerCarry.goals.Add(fJumpGoal);
		cornerCarry.goals.Add(adGoal);
		cornerCarry.goals.Add(j2sGoal);
		cornerCarry.goals.Add(slashGoal);
		cornerCarry.goals.Add(sixCGoal);
		cornerCarry.goals.Add(cslashGoal);
		cornerCarry.MakeComboChallenge();
		challenges.Add(cornerCarry);

		Challenge cornerSweep = new Challenge("Corner Sweep Extension", GameScene.ResetPos.P2CORNEREDRIGHT);
		cornerSweep.goals.Add(cjabGoal);
		cornerSweep.goals.Add(ckickGoal);
		cornerSweep.goals.Add(cslashGoal);
		cornerSweep.goals.Add(jrGoal);
		cornerSweep.goals.Add(jabGoal);
		cornerSweep.goals.Add(slashGoal);
		cornerSweep.goals.Add(sixCGoal);
		cornerSweep.goals.Add(cslashGoal);
		cornerSweep.MakeComboChallenge();
		challenges.Add(cornerSweep);
	}

	protected void AddSLChallenges()
	{

		RecordingName = "SL_combos";
		Goal sixPGoal = new Goal("Uppercut", "right", "p")
		{
			p2StateFrame = 0,
			p1State = "6P"
		};

		Goal sixCGoal = new Goal("Heavy Slash", "right", "s")
		{
			p2StateFrame = 0,
			p1State = "6C"
		};

		Goal sixCHoldGoal = new Goal("Heavy Slash", "right", "s", "hold")
		{
			p2StateFrame = 0,
			p1State = "6CH"
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

		Goal jKick2HitsGoal = new Goal("Aerial Kick (2 hits)", "air", "k")
		{
			p2StateFrame = 0,
			p1State = "JumpB"
		};


		Challenge basicComboChallenge = new Challenge("Easy Combo");
		basicComboChallenge.goals.Add(jabGoal);
		basicComboChallenge.goals.Add(kickGoal);
		basicComboChallenge.goals.Add(slashGoal);
		basicComboChallenge.goals.Add(cslashGoal);
		basicComboChallenge.MakeComboChallenge();
		challenges.Add(basicComboChallenge);

		Challenge airConfirm = new Challenge("Air combo");
		airConfirm.goals.Add(sixPGoal);
		airConfirm.goals.Add(fJumpGoal);
		airConfirm.goals.Add(jKick2HitsGoal);
		airConfirm.goals.Add(jJabGoal);
		airConfirm.goals.Add(j2CGoal);
		airConfirm.MakeComboChallenge();
		challenges.Add(airConfirm);

		Challenge bigCornerDamage = new Challenge("Big damage");
		bigCornerDamage.goals.Add(sixPGoal);
		bigCornerDamage.goals.Add(sixCHoldGoal);
		bigCornerDamage.goals.Add(phoneTossGoal);
		bigCornerDamage.goals.Add(walkGoal);
		bigCornerDamage.goals.Add(kickGoal);
		bigCornerDamage.goals.Add(sixCHoldGoal);
		bigCornerDamage.goals.Add(phoneTossGoal);
		bigCornerDamage.goals.Add(cslashGoal);
		bigCornerDamage.MakeComboChallenge();
		challenges.Add(bigCornerDamage);
	}
}
