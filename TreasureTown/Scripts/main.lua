--This is the main game loop, using a modified scheduler.
require "Scripts/scheduler"

luanet.load_assembly "TreasureTown"
Event = luanet.import_type "TreasureTown.EventManager"
GRAPHIC_ELEMENT = luanet.import_type "TreasureTown.GraphicElement"

gameFinished = false

-- This must be run as a coroutine
function mainGameLoop()
	
	--Setup the game
	initialGameSetup()
	
	while not gameFinished do

		--Set the team whose turn is next and begin the turn
		nextTeam()

		--Suspend the main loop
		sleepUntil(currentTeam.Name.." turn finished.")

		--Check to see if the conditions for game over have been met
		checkGameOver()
		if not gameFinished then
			-- If the game hasn't ended, see if the game needs to shift
			checkShift()
			sleepUntil("Shift check finished.")
		end
		
	end

	--Go to the results screen
	goToResults()
end


function checkGameOver()

	if treasureFound or timeOver or earlyAbort then

		gameFinished = true;

	else

		gameFinished = false;

	end

end

function goStraight()
	runCoroutine(function()
		Event.DisableInput()
		Event.GoStraight()
		waitUntil("PLAYER position lerped")
		Event.CheckIfExploring()
	end)
end

function turnLeft()
	runCoroutine(function()
		Event.DisableInput()
		Event.TurnLeft()
		waitUntil("PLAYER rotation lerped")
		Event.EnableInput()
	end)
end

function turnRight()
	runCoroutine(function()
		Event.DisableInput()
		Event.TurnRight()
		waitUntil("PLAYER rotation lerped")
		Event.EnableInput()
	end)
end

function nextTeam()
	runCoroutine(function()
		Event.GetNextTeam()
		
		--Show a banner showing the team's name
		Event.CreateBannerGraphic("Graphics/GUI/team"..(Event.GetCurrentTeamIndex() + 1).."banner", -600, 0, "TeamBanner")
		
		Event.PlaySoundEffect("whoosh", .7);
		Event.LerpPosition("TeamBanner", 0, 0, .5)
		waitSeconds(2.25)
		Event.LerpPosition("TeamBanner", 800, 0, .75)
		waitSeconds(.5)
		Event.DestroyGraphic("TeamBanner")
		
		if Event.CurrentTeamHasMap() then
			showMap()
		end
		
		Event.StartTurn()
	end)
end

function showMap()
	Event.SetCurrentTeamMap(false)
	Event.CreateMapGraphic(0,0,"MAP")
	Event.PlaySoundEffect("openmap", 1);
	Event.LerpColor("MAP", 1,1,1,1, 2)
	waitSeconds(6)
	Event.LerpColor("MAP", 0,0,0,0, 1.5)
	Event.PlaySoundEffect("closemap", 1);
	waitSeconds(1.5)
	Event.DestroyGraphic("MAP")
end

function delayPointScroll()
	runCoroutine(function()
		waitSeconds(1.5)
		Event.ScrollPoints()
	end)
end

function endTurn()
	runCoroutine(function()
		Event.EndTurn()
		checkChange()
		Event.Write("Change checked raised!")
		if not Event.CheckEndOfGame() then
			nextTeam()
		end
	end)	
end


function checkChange()
	
	if Event.ChangeFilled() then
		Event.Write("CHANGE IS FULL!!")
		runChange()
		Event.Write("Change completed raised!")
	end
	Event.Write("leaving checkChange()")
	Event.SendSignal("Change checked")
end



function runChange()
	Event.Write("run change()")
	--yes we can
	--Randomize the change mode and set it in the game scene
	type = Event.GetChangeMode()
	showChangeGraphic(type)
	Event.ResetChange()
end



function showChangeGraphic(type)
	Event.FadeOutMusic(2)

	Event.Write("run showChangeGraphic()")
	Event.PlaySoundEffect("change", 1);
	Event.CreateGraphic("Graphics/GUI/change", 0, 0, "CHANGE")
	Event.LerpColor("CHANGE", 1, 1, 1, 1, .5)
	waitSeconds(2)
	
	Event.KillMusic()
	-- Do this here, for some reason
	Event.GenerateTown()
		
	Event.LerpColor("CHANGE", 0, 0, 0, 0, .5)
	waitSeconds(.5)
	Event.DestroyGraphic("CHANGE")
		
		
	if type == "Normal" then
		Event.PlaySong("TreasureHunt")
		--Do nothing!
	elseif type == "Danger" then
		Event.PlaySong("BakugekiNights")
		Event.CreateGraphic("Graphics/GUI/DangerBanner", 0, 0, "BOMB")
		Event.LerpColor("BOMB", 1, 1, 1, 1, .5)
		waitSeconds(2.5)
		Event.LerpColor("BOMB", 0, 0, 0, 0, .5)
		waitSeconds(.5)
		Event.DestroyGraphic("BOMB")
	elseif type == "CrimeWave" then
		--Play crimewave sound
		Event.PlaySong("DorobonusRound")
		Event.CreateGraphic("Graphics/GUI/CrimeWaveBanner", 0, 0, "CRIME")
		Event.LerpColor("CRIME", 1, 1, 1, 1, .5)
		waitSeconds(2.5)
		Event.LerpColor("CRIME", 0, 0, 0, 0, .5)
		waitSeconds(.5)
		Event.DestroyGraphic("CRIME")
	elseif type == "Greed" then
		Event.PlaySong("PointsOrDie")
		--Play greed sound
		Event.CreateGraphic("Graphics/GUI/GreedBanner", 0, 0, "GREED")
		Event.LerpColor("GREED", 1, 1, 1, 1, .5)
		waitSeconds(2.5)
		Event.LerpColor("GREED", 0, 0, 0, 0, .5)
		waitSeconds(.5)
		Event.DestroyGraphic("GREED")
	end
end

function showExplosion()
	-- PLAY THE SOUND EFFECT!
	Event.PlaySoundEffect("bomb");
	Event.CreateExplosionGraphic()
	Event.LerpColor("EXPLOSION", 1, 1, 1, 1, .5)
	Event.LerpScale("EXPLOSION", 1, .5)
	waitSeconds(.5)
	Event.LerpScale("EXPLOSION", 1.35, 2)
	Event.LerpColor("EXPLOSION", 0, 0, 0, 0, 2)
	waitSeconds(2)
	Event.DestroyGraphic("EXPLOSION")
end

function endGame()
	runCoroutine(function()
	
		-- Fade out the music over a few seconds.
		Event.FadeOutMusic(3)
		waitSeconds(3)
		Event.CreateGraphic("Graphics/GUI/Finish", 0, 0, "FINISH")
		-- Play a cool sound?
		Event.LerpColor("FINISH", 1, 1, 1, 1, 1.5)
		waitSeconds(4)
		Event.GoToResultsScene()
				
	end)
end