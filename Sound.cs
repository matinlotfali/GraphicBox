using System;

using System.Media;
using WMPLib;
using System.Collections.Generic;
using System.IO;
using Toub.Sound.Midi;

namespace GraphDLL
{
    public static partial class Graph
    {
        static WindowsMediaPlayer player;
        static List<string> notesPlayed;

        static void initsound()
        {
            player = new WindowsMediaPlayer();
        }

        static void OpenMidi()
        {
            if (notesPlayed == null)
            {
                MidiPlayer.OpenMidi();
                notesPlayed = new List<string>();
            }
        }

        public static void playsound(string filepath)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException();
            player.URL = filepath;
            while (true)
                try
                {
                    if (player.playState == WMPPlayState.wmppsPlaying)
                        break;
                }
                catch { }
        }

        public static void nosound()
        {
            if (player != null)
                player.controls.stop();

            if (notesPlayed != null)
            {
                foreach (string n in notesPlayed)
                    MidiPlayer.Play(new NoteOff(0, 1, n, 100));
                notesPlayed.Clear();
            }
        }

        public static bool isplaying
        {
            get
            {
                if (player != null)
                    if (player.playState == WMPPlayState.wmppsPlaying)
                        return true;
                if (notesPlayed != null)
                    if (notesPlayed.Count > 0)
                        return true;
                return false;
            }
        }

        public static void playwave(string wavepath)
        {
            node p = Queues.SearchFile(wavepath);
            if (p.data == null)
                p.data = new SoundPlayer(wavepath);
            ((SoundPlayer)(p.data)).Play();
        }

        public static void sound(string note)
        {
            OpenMidi();
            MidiPlayer.Play(new NoteOn(0, 1, note, 100));
            if (!notesPlayed.Contains(note))
                notesPlayed.Add(note);
        }

        public static void nosound(string note)
        {
            OpenMidi();
            MidiPlayer.Play(new NoteOff(0, 1, note, 100));
            notesPlayed.Remove(note);
        }

        public static void setinstrument(MIDIInstruments instrument)
        {
            OpenMidi();
            MidiPlayer.Play(new ProgramChange(0, 1, (GeneralMidiInstruments)instrument));
        }
    }
}
