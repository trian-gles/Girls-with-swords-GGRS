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

	public override void Start()
	{
		comboTrial = true;
		base.Start();
	}

	protected void AddOLChallenges()
	{
		RecordingName = "OL_combos";
		Goal chargedHojoGoal = new Goal("Hojogiri, full charge", "qcf", "k", "hold");
		// needs to be completed

		Goal dpGoal = new Goal("Dragon Punch", "dp", "s")
		{
			p2StateFrame = 0,
			p1State = "AntiAir"
		};

		Goal airDpGoal = new Goal("Dragon Punch", "air", "dp", "s")
		{
			p2StateFrame = 0,
			p1State = "AntiAir"
		};

		Goal hojogiriGoal = new Goal("Hojogiri", "qcf", "k")
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

		Goal hadoukenGoal = new Goal("Slow Coffee", "qcf", "p")
		{
			p2StateFrame = 0,
			p1State = "Hadouken"
		};
		Goal runGoal = new Goal("Run", "right", "dash");
		runGoal.p1State = "Run";


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
		basicAirCombo.goals.Add(airDpGoal);
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
		cornerThrowCombo.goals.Add(kickGoal);
		cornerThrowCombo.goals.Add(sixSGoal);
		cornerThrowCombo.goals.Add(hojogiriGoal);
		cornerThrowCombo.goals.Add(cjabGoal);
		cornerThrowCombo.goals.Add(kickGoal);
		cornerThrowCombo.goals.Add(sixSGoal);
		cornerThrowCombo.goals.Add(hojogiriGoal);
		cornerThrowCombo.MakeComboChallenge();

		challenges.Add(cornerThrowCombo);
		

		Challenge cornerPunish = new Challenge("Big corner punish", GameScene.ResetPos.P2CORNEREDRIGHT);

		cornerPunish.goals.Add(sixPGoal);
		cornerPunish.goals.Add(sixSGoal);
		cornerPunish.goals.Add(hadoukenGoal);
		cornerPunish.goals.Add(runGoal);
		cornerPunish.goals.Add(kickGoal);
		cornerPunish.goals.Add(sixSGoal);
		cornerPunish.goals.Add(hadoukenGoal);
		cornerPunish.goals.Add(sixPGoal);
		cornerPunish.goals.Add(fJumpGoal);
		cornerPunish.goals.Add(adGoal);
		cornerPunish.goals.Add(jJabGoal);
		cornerPunish.goals.Add(airDpGoal);
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

		Goal sixPGoal = new Goal("High Kick", "right", "p")
		{
			p2StateFrame = 0,
			p1State = "3K"
		};

		Goal j2CGoal = new Goal("Downward Aerial Slash", "air", "down", "s")
		{
			p2StateFrame = 0,
			p1State = "J2C"
		};

		Goal gunBlazedGoal = new Goal("Gunblazed", "qcf", "p")
		{
			p2StateFrame = 0,
			p1State = "GunBlazed"
		};

		Goal superGoal = new Goal("OH SHIT", "qcf", "qcf", "s")
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
		meterExtendedComboChallenge.goals.Add(sixPGoal);
		meterExtendedComboChallenge.goals.Add(superGoal);
		meterExtendedComboChallenge.MakeComboChallenge();

		challenges.Add(meterExtendedComboChallenge);

		Challenge cornerComboChallenge = new Challenge("Corner combo extension", GameScene.ResetPos.P2CORNEREDRIGHT);
		cornerComboChallenge.goals.Add(cslashGoal);
		cornerComboChallenge.goals.Add(gunBlazedGoal);
		Goal runGoal = new Goal("Run", "right", "dash");
		cornerComboChallenge.goals.Add(runGoal);
		cornerComboChallenge.goals.Add(dashAttackGoal);
		cornerComboChallenge.goals.Add(cslashGoal);
		cornerComboChallenge.MakeComboChallenge();
		challenges.Add(cornerComboChallenge);

		Challenge extendedComboChallenge = new Challenge("Hard Meterless Corner Carry Combo");
		dFJumpGoal.p1StateFrame = 1;
		dFJumpGoal.p1Tags = new HashSet<Globals.Tags> { Globals.Tags.aerial };
		dFJumpGoal.p1State = null;
		extendedComboChallenge.goals.Add(slashGoal);
		extendedComboChallenge.goals.Add(sixSGoal);
		extendedComboChallenge.goals.Add(adGoal);
		extendedComboChallenge.goals.Add(jSlashGoal);
		extendedComboChallenge.goals.Add(sixPGoal);
		extendedComboChallenge.goals.Add(fJumpGoal);
		extendedComboChallenge.goals.Add(jKickGoal);
		extendedComboChallenge.goals.Add(jJabGoal);
		extendedComboChallenge.goals.Add(jKickGoal);
		extendedComboChallenge.goals.Add(dFJumpGoal);
		extendedComboChallenge.goals.Add(j2CGoal);
		extendedComboChallenge.goals.Add(jSlashGoal);
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

		Goal sixKGoal = new Goal("Forward Kick", "right", "k")
		{
			p2StateFrame = 0,
			p1State = "6K"
		};

		Goal sixCGoal = new Goal("Heavy Slash", "right", "s")
		{
			p2StateFrame = 0,
			p1State = "6S"
		};

		Goal slash2xGoal = new Goal("Slash (2 hits)", "s")
		{
			p2StateFrame = 0,
			p1State = "Slash"
		};

		Goal j2sGoal = new Goal("Down Slash", "air", "down", "s")
		{
			p2StateFrame = 0,
			p1State = "J2C"
		};

		Goal jrGoal = new Goal("Wheeeeee", "air", "qcb", "p")
		{
			p2StateFrame = 0,
			p1State = "JoeRogan"
		};

		Goal groundjrGoal = new Goal("Wheeeeee", "qcb", "p")
		{
			p2StateFrame = 0,
			p1State = "JoeRogan"
		};

		Goal superGoal = new Goal("OH SHIT", "qcf", "qcf", "s")
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
		cornerCarry.goals.Add(kickGoal);
		cornerCarry.goals.Add(ckickGoal);
		cornerCarry.goals.Add(sixPGoal);
		cornerCarry.goals.Add(sixKGoal);
		cornerCarry.goals.Add(groundjrGoal);
		cornerCarry.goals.Add(jabGoal);
		cornerCarry.goals.Add(slashGoal);
		cornerCarry.goals.Add(sixCGoal);
		cornerCarry.goals.Add(cslashGoal);
		cornerCarry.MakeComboChallenge();
		challenges.Add(cornerCarry);

		Goal hatUpGoal = new Goal("Eat a hat (up)", "qcf", "s")
		{
			p1State = "UpHat",
			p2StateFrame = 0
		};

		Goal sJumpGoal = new Goal("Super Jump", "up", "dash")
		{
			p1State = "SuperJump"	
		};

		Goal teleportDPGoal = new Goal("Suprise! (Up)", "dp", "s")
		{
			p1State = "TeleportDP"
		};

		Challenge cornerSweep = new Challenge("Corner Sweep Extension", GameScene.ResetPos.P2CORNEREDRIGHT);
		cornerSweep.goals.Add(cslashGoal);
		cornerSweep.goals.Add(groundjrGoal);
		cornerSweep.goals.Add(jabGoal);
		cornerSweep.goals.Add(sixPGoal);
		cornerSweep.goals.Add(sJumpGoal);
		cornerSweep.goals.Add(jSlashGoal);
		cornerSweep.goals.Add(j2sGoal);
		cornerSweep.goals.Add(slash2xGoal);
		cornerSweep.goals.Add(sixCGoal);
		cornerSweep.goals.Add(cslashGoal);
		cornerSweep.MakeComboChallenge();
		challenges.Add(cornerSweep);

		Challenge hugePunish = new Challenge("Huge Punish");
		hugePunish.goals.Add(slashGoal);
		hugePunish.goals.Add(sixPGoal);
		hugePunish.goals.Add(hatUpGoal);
		hugePunish.goals.Add(teleportDPGoal);
		hugePunish.goals.Add(sixPGoal);
		hugePunish.goals.Add(sJumpGoal);
		hugePunish.goals.Add(jSlashGoal);
		hugePunish.goals.Add(j2sGoal);
		hugePunish.goals.Add(slashGoal);
		hugePunish.goals.Add(sixCGoal);
		hugePunish.goals.Add(cslashGoal);
		hugePunish.MakeComboChallenge();
		challenges.Add(hugePunish);
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

		Goal sixCHoldGoal = new Goal("2x Heavy Slash", "right", "s", "hold")
		{
			p2StateFrame = 0,
			p1State = "6CH"
		};


		Goal phoneTossGoal = new Goal("It's for you", "qcf", "p")
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

		Goal slash3HitsGoal = new Goal("Slash (3 hits)", "s")
		{
			p2StateFrame = 0,
			p1State = "Slash"
		};

		Goal dashAttackGoal = new Goal("Dash Attack", "right", "s")
		{
			p2StateFrame = 0,
			p1State = "DashAttack"
		};

		Goal runGoal = new Goal("Run", "right", "dash");
		runGoal.p1State = "Run";


		Challenge basicComboChallenge = new Challenge("Universal Combo");
		basicComboChallenge.goals.Add(cjabGoal);
		basicComboChallenge.goals.Add(kickGoal);
		basicComboChallenge.goals.Add(slash3HitsGoal);
		basicComboChallenge.goals.Add(sixCHoldGoal);
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

		Challenge snailSetup = new Challenge("Snail setup");
		snailSetup.goals.Add(ckickGoal);
		snailSetup.goals.Add(sixPGoal);
		snailSetup.goals.Add(slashGoal);
		snailSetup.goals.Add(sixCGoal);
		snailSetup.goals.Add(phoneTossGoal);
		snailSetup.goals.Add(runGoal);
		snailSetup.goals.Add(dashAttackGoal);
		snailSetup.goals.Add(kickGoal);
		snailSetup.goals.Add(sixCHoldGoal);
		snailSetup.goals.Add(cslashGoal);
		snailSetup.MakeComboChallenge();
		challenges.Add(snailSetup);

		Challenge bigCornerDamage = new Challenge("Corner double setup", GameScene.ResetPos.P2CORNEREDRIGHT);
		bigCornerDamage.goals.Add(ckickGoal);
		bigCornerDamage.goals.Add(sixPGoal);
		bigCornerDamage.goals.Add(slashGoal);
		bigCornerDamage.goals.Add(sixCGoal);
		bigCornerDamage.goals.Add(phoneTossGoal);
		bigCornerDamage.goals.Add(dashAttackGoal);
		bigCornerDamage.goals.Add(sixPGoal);
		bigCornerDamage.goals.Add(sixCGoal);
		bigCornerDamage.goals.Add(phoneTossGoal);
		bigCornerDamage.goals.Add(dashAttackGoal);
		bigCornerDamage.goals.Add(kickGoal);
		bigCornerDamage.goals.Add(sixCHoldGoal);
		bigCornerDamage.goals.Add(cslashGoal);
		bigCornerDamage.MakeComboChallenge();
		challenges.Add(bigCornerDamage);
	}
}
