using Godot;
using System.Collections.Generic;


public class StrategyManager : TutorialManager
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

	protected void AddOLChallenges()
	{
		RecordingName = "OL_strategy";
		Goal chargedHojoGoal = new Goal("Hojogiri, full charge", "qcf", "k", "hold")
		{
			p2StateFrame = 0,
			p1State = "HojogiriChargedSlash"
		};

		Goal reverseChargedHojoGoal = new Goal("Cross up Hojogiri", "qcf", "s")
		{
			p1State = "CommandRunTurn"
		};

		Goal coffeeGoal = new Goal("Slow Bean Juice", "qcf", "p")
		{
			p2StateFrame = 0,
			p1State = "Hadouken"
		};

		Goal fastCoffeeGoal = new Goal("Fast Bean Juice", "qcb", "p")
		{
			p2StateFrame = 0,
			p1State = "FastHadouken"
		};

		Goal airCoffeeGoal = new Goal("Air Bean Juice", "air", "qcf", "p")
		{
			p2StateFrame = 0,
			p1State = "AirHadouken"
		};

		Goal dpGoal = new Goal("Dragon Punch", "dp", "s")
		{
			p2StateFrame = 0,
			p1State = "AntiAir",
			p1FailTags = new HashSet<Globals.Tags> { Globals.Tags.hitstate }
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



        Goal dashAttackGoal = new Goal("Dashing slash", "right", "dash", "hold", "s")
        {
            p1State = "InstantOverhead"
        };

        Challenge hojogiriChallenge = new Challenge("Hojogiri");
		hojogiriChallenge.popupText = "OL's advancing attack \"Hojogiri\" is normally dangerous against a blocking opponent, but can fake them out with the two effects.";
		hojogiriChallenge.goals.Add(hojogiriGoal);
		hojogiriChallenge.goals.Add(chargedHojoGoal);
		hojogiriChallenge.goals.Add(reverseChargedHojoGoal);
		challenges.Add(hojogiriChallenge);

		Challenge dpChallenge = new Challenge("Dragon Punch", GameScene.ResetPos.P1CORNEREDLEFT);
		dpChallenge.p2Inputs = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 520, 8, 8, 8, 8, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 66, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 40, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		dpChallenge.popupText = "OL possesses a extremely strong tool called a \"Dragon Punch\", which is invincible during it's early frames.  It can be done on the ground or in the air.  Use this to punish an overly aggressive opponent, like GL in this situation.  ";
		dpChallenge.goals.Add(dpGoal);
		challenges.Add(dpChallenge);
		//dpChallenge.goals.Add(jdpGoal);


		Challenge coffeeThrowChallenge = new Challenge("Overcaffinated");
		coffeeThrowChallenge.popupText = "While it isn't the most powerful projectile, OL's boiling hot coffee can help lock down the opponent or destroy other dangerous projectiles";
		coffeeThrowChallenge.goals.Add(coffeeGoal);
		coffeeThrowChallenge.goals.Add(fastCoffeeGoal);
		coffeeThrowChallenge.goals.Add(airCoffeeGoal);
		challenges.Add(coffeeThrowChallenge);

		Challenge dashAttackChallenge = new Challenge("Overhead Dash Attack");
		dashAttackChallenge.popupText = "OL's dash attack is a fast overhead that can surprise the opponent";
		dashAttackChallenge.goals.Add(dashAttackGoal);
		challenges.Add(dashAttackChallenge);
		



	}

	protected void AddGLChallenges()
	{
		RecordingName = "GL_strategy";
		Goal lowFireGoal = new Goal("Low fireball", "qcf", "k")
		{
			p1State = "Hadouken"
		};

		Goal arcFireGoal = new Goal("Arc fireball", "dp", "s")
		{
			p1State = "HadoukenAir"
		};

		Goal feintGoal = new Goal("Feint fireball", "qcf", "s")
		{
			p1State = "Feint"
		};

		Goal airFireGoal = new Goal("Air fireball", "air", "qcf", "k")
		{
			p1State = "HadoukenAirDown"
		};

		Goal gunblazedGoal = new Goal("Gunblazed", "down", "special")
		{
			p1State = "GunBlazed"
		};

		Challenge fireballChallenge = new Challenge("Fireballs");
		fireballChallenge.popupText = "GL's fireballs can chip away at the opponent's health while keeping them out of range.  Mix it up between a bunch of options.";
		fireballChallenge.goals.Add(lowFireGoal);
		fireballChallenge.goals.Add(arcFireGoal);
		fireballChallenge.goals.Add(feintGoal);
		fireballChallenge.goals.Add(airFireGoal);
		challenges.Add(fireballChallenge);

		Goal sixKGoal = new Goal("Forward kick", "right", "k")
		{
			p1State = "6K"
		};


		Goal blackHoleGoal = new Goal("Come with me", "air", "dp", "s")
		{
			p1State = "BlackHolePlace"
		};
		Challenge blackHoleChallenge = new Challenge("Portal");
		blackHoleChallenge.popupText = "GL's portal pulls in nearby aerial opponents.  Use this to cripple an opponent's escape options and then score an extended combo.";
		blackHoleChallenge.goals.Add(blackHoleGoal);
		challenges.Add(blackHoleChallenge);



		Challenge closeChallenge = new Challenge("Close range mixup");
		closeChallenge.popupText = "On the offense in close range, GL can mixup the opponent with her overhead forward kick and her low crouching kick";
		closeChallenge.goals.Add(sixKGoal);
		closeChallenge.goals.Add(ckickGoal);
		challenges.Add(closeChallenge);
		
		Challenge halfscreenChallenge = new Challenge("Longer range");
		halfscreenChallenge.popupText = "GL generally applies pressure from a distance, choosing between a fireball, running in for a mixup, or her explosive gunblazed attack";
		halfscreenChallenge.goals.Add(gunblazedGoal);
		challenges.Add(halfscreenChallenge);
	}

	protected void AddHLChallenges()
	{
		RecordingName = "HL_strategy";
		Goal hatGoal = new Goal("Eat a hat", "qcf", "k")
		{
			p1State = "Hadouken"
		};
		Goal hatUpGoal = new Goal("Eat a hat (up)", "qcf", "p")
		{
			p1State = "UpUpHat"
		};

		Goal hatUpUpGoal = new Goal("Eat a hat (far)", "qcf", "s")
		{
			p1State = "UpHat"
		};

		Goal dpGoal = new Goal("HAT IN YOUR FACE", "dp", "s")
		{
			p1State = "DP"
		};

		Goal teleportGoal = new Goal("Teleport", "qcf", "k")
		{
			p1State = "Teleport"
		};

		Goal teleportDownSlashGoal = new Goal("Suprise! (Down)", "qcf", "p")
		{
			p1State = "TeleportDownSlash"
		};

		Goal teleportDPGoal = new Goal("Suprise! (Up)", "dp", "s")
		{
			p1State = "TeleportDP"
		};

		Goal hatProjectileGoal = new Goal("Suprise?", "qcf", "s")
		{
			p1State = "HatSlash"
		};

		Challenge hatChallenge = new Challenge("The Art of Hat");
		hatChallenge.popupText = "HL can use her hat as a projectile and to create a beacon to teleport to.  Use this to fly around the screen, reset pressure, and mixup the opponent";
		hatChallenge.goals.Add(hatGoal);
		hatChallenge.goals.Add(teleportGoal);
		hatChallenge.goals.Add(hatUpGoal);
		hatChallenge.goals.Add(teleportDownSlashGoal);
		hatChallenge.goals.Add(hatUpUpGoal);
		hatChallenge.goals.Add(teleportDPGoal);
		hatChallenge.goals.Add(hatGoal);
		hatChallenge.goals.Add(hatProjectileGoal);
		challenges.Add(hatChallenge);

		Challenge hatMoveChallenge = new Challenge("Hats on, Hats off");
		hatMoveChallenge.popupText = "HL's gameplay changes significantly when she removes her hat - many of her attacks become useless.  Her KICK and SLASH buttons now move the hat around.";

		hatMoveChallenge.goals.Add(hatGoal);
		hatMoveChallenge.goals.Add(kickGoal);
		hatMoveChallenge.goals.Add(slashGoal);
		challenges.Add(hatMoveChallenge);

		Challenge dpChallenge = new Challenge("DP");
		dpChallenge.popupText = "While wearing her hat, HL can use her invincible Dragon Punch to escape pressure.  If you hold the slash button, she will then deploy her hat.";
		challenges.Add(dpChallenge);

		
	}

	protected void AddSLChallenges()
	{
		RecordingName = "SL_strategy";
		Goal sixKGoal = new Goal("Forward kick", "right", "k")
		{
			p1State = "6K"
		};

		Goal snailGoal = new Goal("Let's go girls", "left", "special")
		{
			p1State = "BackToss"
		};

		Goal airTossGoal = new Goal("Let's go girls (air)", "air", "special")
		{
			p1State = "SnailCallJump"
		};

		Goal groundSnailGoal = new Goal("1-800-SLIMESMACK", "special")
		{
			p1State = "SnailCall"
		};

		Goal airSnailGoal = new Goal("1-800-SHELLSMASH", "right", "special")
		{
			p1State = "SnailCallJump"
		};

		Goal fakeTossGoal = new Goal("It's for you", "down", "special")
		{
			p1State = "PhoneToss"
		};

		Goal snailAirSpecial = new Goal("Big Flop", "air", "special")
		{
			p1State = "SnailCallJump"
		};
		Challenge snailChallenge = new Challenge("Calling in the girls");
		snailChallenge.popupText = "SL relies on her shelled buddies to win the match.  They must be first deployed and then commanded.";
		snailChallenge.goals.Add(snailGoal);
		snailChallenge.goals.Add(groundSnailGoal);
		snailChallenge.goals.Add(airTossGoal);
		snailChallenge.goals.Add(airSnailGoal);
		challenges.Add(snailChallenge);
		

		Challenge phoneTossChallenge = new Challenge("Faking a snail command");
		phoneTossChallenge.popupText = "SL can pretend to call in a snail but instead throw her phone at the opponent.";
		phoneTossChallenge.goals.Add(fakeTossGoal);
		challenges.Add(phoneTossChallenge);


		Challenge highlowChallenge = new Challenge("High Low mixup");
		highlowChallenge.popupText = "SL can mixup between her overhead forward kick and her low crouching kick, however the former requires support from a snail or a Force Cancel to turn into a combo";
		highlowChallenge.goals.Add(ckickGoal);
		highlowChallenge.goals.Add(sixKGoal);
		challenges.Add(highlowChallenge);

	}	
}
