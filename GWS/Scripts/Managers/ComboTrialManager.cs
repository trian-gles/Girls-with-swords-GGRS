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
				AddHLChallenges();
				return;
			case 3:
				AddGLChallenges();
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

		Challenge cornerThrowCombo = new Challenge("Corner throw", GameScene.ResetPos.P2CORNEREDRIGHT);
		cornerThrowCombo.goals.Add(grabGoal);
		cornerThrowCombo.goals.Add(sixSGoal);
		cornerThrowCombo.goals.Add(dpGoal);
		cornerThrowCombo.goals.Add(sixSGoal);
		cornerThrowCombo.goals.Add(hojogiriGoal);
		cornerThrowCombo.goals.Add(cjabGoal);
		cornerThrowCombo.goals.Add(dpGoal);
		cornerThrowCombo.goals.Add(slashGoal);
		cornerThrowCombo.goals.Add(hojogiriGoal);
		cornerThrowCombo.MakeComboChallenge();

		challenges.Add(cornerThrowCombo);

		Challenge midScreenPunish = new Challenge("Midscreen confirm/punish");
		midScreenPunish.goals.Add(cjabGoal);
		midScreenPunish.goals.Add(sixPGoal);
		midScreenPunish.goals.Add(fJumpGoal);
		midScreenPunish.goals.Add(jSlashGoal);
		midScreenPunish.goals.Add(dpGoal);
		midScreenPunish.goals.Add(hojogiriGoal);
		midScreenPunish.MakeComboChallenge();

		challenges.Add(midScreenPunish);

		Challenge cornerPunish = new Challenge("Corner confirm/punish", GameScene.ResetPos.P2CORNEREDRIGHT);
		cornerPunish.goals.Add(cjabGoal);
		cornerPunish.goals.Add(sixPGoal);
		cornerPunish.goals.Add(fJumpGoal);
		cornerPunish.goals.Add(jKickGoal);
		cornerPunish.goals.Add(dpGoal);
		cornerPunish.goals.Add(sixSGoal);
		cornerPunish.goals.Add(hojogiriGoal);
		cornerPunish.goals.Add(cjabGoal);
		cornerPunish.goals.Add(dpGoal);
		cornerPunish.goals.Add(slashGoal);
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
		challenges.Add(basicComboChallenge);

		Goal j2CGoal = new Goal("Downward Aerial Slash", "air", "down", "s")
		{
			p2StateFrame = 0,
			p1State = "J2C"
		};

		Goal gunBlazedGoal = new Goal("Gunblazed", "down", "special")
		{
			p2StateFrame = 1,
			p1State = "GunBlazed"
		};

		Goal superGoal = new Goal("Super", "right", "s", "special")
		{
			p2StateFrame = 0,
			p1State = "GLDP"
		};

		Challenge meterExtendedComboChallenge = new Challenge("Extended Metered Combo");
		meterExtendedComboChallenge.goals.Add(ckickGoal);
		meterExtendedComboChallenge.goals.Add(slashGoal);
		meterExtendedComboChallenge.goals.Add(sixSGoal);
		meterExtendedComboChallenge.goals.Add(fJumpGoal);
		meterExtendedComboChallenge.goals.Add(adGoal);
		meterExtendedComboChallenge.goals.Add(jSlashGoal);
		meterExtendedComboChallenge.goals.Add(jabGoal);
		meterExtendedComboChallenge.goals.Add(superGoal);
		challenges.Add(meterExtendedComboChallenge);

		Challenge extendedComboChallenge = new Challenge("HARD Corner Carry Combo");
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
		extendedComboChallenge.goals.Add(gunBlazedGoal);
		extendedComboChallenge.goals.Add(cslashGoal);
		challenges.Add(extendedComboChallenge);



	}

	protected void AddHLChallenges()
	{
		Challenge basicComboChallenge = new Challenge("Universal Combo");
		basicComboChallenge.goals.Add(jabGoal);
		basicComboChallenge.goals.Add(kickGoal);
		basicComboChallenge.goals.Add(slashGoal);
		basicComboChallenge.goals.Add(cslashGoal);
	}

	protected void AddSLChallenges()
	{
		Goal sixPGoal = new Goal("Uppercut", "right", "p")
		{
			p2StateFrame = 0,
			p1State = "6P"
		};

		Goal j2CGoal = new Goal("Downward Aerial Slash", "air", "down", "s")
		{
			p2StateFrame = 0,
			p1State = "J2C"
		};


		Challenge basicComboChallenge = new Challenge("Universal Combo");
		basicComboChallenge.goals.Add(jabGoal);
		basicComboChallenge.goals.Add(kickGoal);
		basicComboChallenge.goals.Add(slashGoal);
		basicComboChallenge.goals.Add(cslashGoal);
		challenges.Add(basicComboChallenge);

		Challenge airConfirm = new Challenge("Air combo into knockdown");
		airConfirm.goals.Add(sixPGoal);
		airConfirm.goals.Add(jKickGoal);
		airConfirm.goals.Add(jJabGoal);
		airConfirm.goals.Add(jKickGoal);
		airConfirm.goals.Add(j2CGoal);
		challenges.Add(basicComboChallenge);
	}
}
