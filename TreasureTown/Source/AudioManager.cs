using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using LuaInterface;
using Eglantine.Engine;

namespace Eglantine
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

		private float sfxVolume = 1f;
		private float musicVolume = 1f;

		public Dictionary<string, SoundEffect> SoundEffects;
		public Dictionary<string, SoundEffect> Songs;

		public List<SoundEffectWrapper> LoopingSoundEffects;
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
			LoopingSoundEffects = new List<SoundEffectWrapper>();
			PlayingSongs = new List<SoundEffectInstance>();
			LoadSoundEffects();
			LoadSongs();
		}

		public void Update (GameTime gameTime)
		{
			if (lerpingMusic)
			{
				musicLerpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
				foreach(SoundEffectInstance s in PlayingSongs)
				{
					s.Volume = startVolume - (startVolume - endVolume) * musicLerpTime / musicLerpDuration;
				}

				if(musicLerpTime >= musicLerpDuration)
				{
					lerpingMusic = false;
					//MediaPlayer.Volume = endVolume;
				}
			}
		}


		public void LoadSoundEffects()
		{
			SoundEffects = new Dictionary<string, SoundEffect>();
			LuaTable soundEffectsTable = Eglantine.MainLua.GetTable("sound_effects_to_load");

			string newSoundEffectName;
			SoundEffect newSoundEffect;
			for(int i = 0; i < soundEffectsTable.Keys.Count; i++)
			{
				try
				{
					newSoundEffectName = (string)soundEffectsTable[i+1];
					newSoundEffect = ContentLoader.Instance.Load<SoundEffect>("Audio/SoundEffects/" + newSoundEffectName);
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
			LuaTable songTable = Eglantine.MainLua.GetTable ("songs_to_load");

			string newSongName;
			SoundEffect newSong;
			for (int i = 0; i < songTable.Keys.Count; i++)
			{
				try
				{
					newSongName = (string)songTable[i+1];
					newSong = ContentLoader.Instance.Load<SoundEffect>("Audio/Music/" + newSongName + ".wav");
					Songs.Add(newSongName, newSong);
				}
				catch(Exception e)
				{
					Console.WriteLine("Error loading song: " + e.ToString());
				}
			}
		}

		public void PlaySoundEffect (string soundEffectName, float volume, float pitch = 0f, float pan = 0f)
		{
			if (SoundEffects.ContainsKey (soundEffectName))
			{
				SoundEffects[soundEffectName].Play(volume * sfxVolume, pitch, pan);
			}
			else
			{
				Console.WriteLine("Sound effect " + soundEffectName + " not found!");
			}
		}

		public void StopSoundEffect (string soundEffectName)
		{
			Console.WriteLine ("Stop all sound effects named " + soundEffectName);
			foreach(SoundEffectWrapper s in LoopingSoundEffects.FindAll (x => x.Name == soundEffectName))
			{
				s.Sound.Stop ();
				LoopingSoundEffects.Remove (s);
			}

		}

		public void PlayLoopingSoundEffect (string soundEffectName, float volume, float pitch = 0f, float pan = 0f)
		{
			if (SoundEffects.ContainsKey (soundEffectName))
			{
				SoundEffectInstance sfx = new SoundEffectInstance(SoundEffects[soundEffectName]);
				sfx.Volume = volume;
				sfx.Pitch = pitch;
				sfx.Pan = pan;
				sfx.IsLooped = true;

				// Add it to the list so it can be aborted later.
				LoopingSoundEffects.Add(new SoundEffectWrapper(soundEffectName, sfx));

				sfx.Play ();
			} else
			{
				Console.WriteLine("Sound effect " + soundEffectName + " not found!");
			}
		}

		public void StopLoopingSoundEffects ()
		{
			foreach(SoundEffectWrapper sfx in LoopingSoundEffects)
				sfx.Sound.Stop ();

			LoopingSoundEffects.Clear ();
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

			// For Windows:
			//MediaPlayer.Stop ();
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

