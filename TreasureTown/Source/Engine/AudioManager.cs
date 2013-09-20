using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using SchedulerTest;

namespace TreasureTown
{
	public class AudioManager
	{
		// Yet another singleton to add to the fire
		private static AudioManager _instance;
		public static AudioManager Instance
		{
			get
			{
				if(_instance == null)
					_instance = new AudioManager();

				return _instance;
			}
		}

		private readonly string[] soundEffectsTable = {
			"bomb",
			"boxring",
			"change",
			"closemap",
			"Extend",
			"gamestart",
			"gainpoints",
			"losepoints",
			"openmap",
			"whoosh"
		};

		private readonly string[] songTable = {
			"BakugekiNights",
			"DorobonusRound",
			"PointsOrDie",
			"TreasureHunt"
		};

		public Dictionary<string, SoundEffect> SoundEffects;
		public Dictionary<string, SoundEffect> Songs;

		public List<SoundEffectWrapper> PlayingSoundEffects;
		public List<SoundEffectInstance> PlayingSongs;

		// For lerping music
		bool lerpingMusic;
		float musicLerpTime;
		float musicLerpDuration;
		float startVolume;
		float endVolume;

		public AudioManager ()
		{
		}

		public void Initialize ()
		{
			PlayingSoundEffects = new List<SoundEffectWrapper>();
			PlayingSongs = new List<SoundEffectInstance>();
			LoadSoundEffects();
			LoadSongs();
		}

		public void Update (GameTime gameTime)
		{
			if (lerpingMusic)
			{
				musicLerpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
				foreach (SoundEffectInstance s in PlayingSongs)
				{
					s.Volume = startVolume - (startVolume - endVolume) * musicLerpTime / musicLerpDuration;
				}

				if (musicLerpTime >= musicLerpDuration)
				{
					lerpingMusic = false;
					//MediaPlayer.Volume = endVolume;
				}
			}

			PlayingSoundEffects.RemoveAll(x => x.Sound.State == SoundState.Stopped);
		}


		public void LoadSoundEffects()
		{
			SoundEffects = new Dictionary<string, SoundEffect>();

			string newSoundEffectName;
			SoundEffect newSoundEffect;
			for(int i = 0; i < soundEffectsTable.Length; i++)
			{
				try
				{
					newSoundEffectName = soundEffectsTable[i];
					newSoundEffect = TreasureTown.StaticContent.Load<SoundEffect>("Audio/SoundEffects/" + newSoundEffectName);
					SoundEffects.Add(newSoundEffectName, newSoundEffect);
				}
				catch(Exception e)
				{
					Console.WriteLine("Error loading sound effect: " + e.ToString());
				}
			}
		}

		public void LoadSongs ()
		{
			Songs = new Dictionary<string, SoundEffect>();

			string newSongName;
			SoundEffect newSong;

			for (int i = 0; i < songTable.Length; i++)
			{
				try
				{
					newSongName = (string)songTable[i];
					newSong = TreasureTown.StaticContent.Load<SoundEffect>("Audio/Music/" + newSongName + ".wav");
					Songs.Add(newSongName, newSong);
				}
				catch(Exception e)
				{
					Console.WriteLine("Error loading song: " + e.ToString());
				}
			}
		}

		public void StopSoundEffect (string soundEffectName)
		{
			foreach(SoundEffectWrapper s in PlayingSoundEffects.FindAll (x => x.Name == soundEffectName))
			{
				s.Sound.Stop ();
				PlayingSoundEffects.Remove (s);
			}
		}

		public void PlaySoundEffect (string soundEffectName, float volume, float pitch = 0f, float pan = 0f)
		{
			if (SoundEffects.ContainsKey (soundEffectName) && PlayingSoundEffects.FindAll(x => x.Name == soundEffectName).Count == 0)
			{
				SoundEffectInstance sfx = new SoundEffectInstance(SoundEffects[soundEffectName]);
				sfx.Volume = volume;
				sfx.Pitch = pitch;
				sfx.Pan = pan;

				// Add it to the list so it can be aborted later.
				PlayingSoundEffects.Add(new SoundEffectWrapper(soundEffectName, sfx));

				sfx.Play ();
			} else
			{
				Console.WriteLine("Sound effect " + soundEffectName + " not found!");
			}
		}

		public void StopLoopingSoundEffects ()
		{
			foreach(SoundEffectWrapper sfx in PlayingSoundEffects)
				sfx.Sound.Stop ();

			PlayingSoundEffects.Clear ();
		}

		public void PlaySong (string songName, float volume, bool looping = true, float pitch = 0f, float pan = 0f)
		{
			if (Songs.ContainsKey (songName))
			{
				SoundEffectInstance song = new SoundEffectInstance(Songs[songName]);
				song.Volume = volume;
				song.Pitch = pitch;
				song.Pan = pan;
				song.IsLooped = true;
				
				// Add it to the list so it can be aborted later.
				PlayingSongs.Add(song);
				song.Play ();
			} else
			{
				Console.WriteLine("Song " + songName + " not found!");
			}
		}

		public void StopMusic ()
		{
			foreach (SoundEffectInstance s in PlayingSongs)
			{
				s.Stop ();
			}
			PlayingSongs.Clear ();
		}

		public void FadeMusic (float targetVolume, float duration)
		{
			if (PlayingSongs.Count > 0)
			{
				startVolume = PlayingSongs[0].Volume;
				endVolume = targetVolume;
				musicLerpDuration = duration;
				musicLerpTime = 0f;
				
				lerpingMusic = true;
			}
		}

		public class SoundEffectWrapper
		{
			public SoundEffectInstance Sound;
			public string Name;

			public SoundEffectWrapper(string name, SoundEffectInstance sound)
			{
				Sound = sound;
				Name = name;
			}
		}
	}
}

