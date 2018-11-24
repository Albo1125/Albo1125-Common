using Rage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Albo1125.Common.CommonLibrary
{
    public class Popup
    {
        private static List<Popup> popupQueue = new List<Popup>();
        private static List<GameFiber> popupFibersToDelete = new List<GameFiber>();
        private static bool cleanGameFibersRunning = false;
        private static void cleanGameFibers()
        {
            cleanGameFibersRunning = true;
            GameFiber.StartNew(delegate
            {
                while (true)
                {
                    GameFiber.Sleep(0);
                    foreach (GameFiber g in popupFibersToDelete.ToArray())
                    {
                        if (g.IsAlive)
                        {
                            g.Abort();
                        }
                        popupFibersToDelete.Remove(g);
                    }
                }
            });
        }

        private static Stopwatch timer = new Stopwatch();

        public List<string> popupLines = new List<string>();

        public string PopupText
        {
            set
            {
                popupLines = value.WrapText(720, "Arial Bold", 15.0f, out PopupTextLineHeight);
                addAnswersToText();
            }
        }
        private List<string> answersAsDisplayed = new List<string>();

        private Double PopupTextLineHeight = 0;
        public string PopupTitle = "";
        public bool PauseGame = false;
        private List<string> Answers = new List<string>();
        public bool showEnterConfirmation = true;
        public bool ForceDisplay = false;
        public int IndexOfGivenAnswer = -1;
        public bool ShuffleAnswers = false;
        public Action<Popup> ShownCallback;
        public int Delay = 0;
        private GameFiber fiber;
        private bool _hasDisplayed = false;
        public bool hasDisplayed
        {
            get
            {
                return _hasDisplayed;
            }
            private set
            {
                this._hasDisplayed = value;
            }
        }

        private bool _isDisplaying = false;
        public bool isDisplaying
        {
            get
            {
                return _isDisplaying;
            }
            private set
            {
                this._isDisplaying = value;
            }
        }

        public Popup() { }
        public Popup(string title, string text, bool pausegame, bool showconfirmation, Action<Popup> showncallback = null, int delay = 0, bool forcedisplay = false)
        {
            this.PopupTitle = title;
            this.PopupText = text;
            this.PauseGame = pausegame;
            this.showEnterConfirmation = showconfirmation;
            this.ForceDisplay = forcedisplay;
            this.ShownCallback = showncallback;
            this.Delay = delay;
            addAnswersToText();
        }

        public Popup(string title, string text, List<string> answers, bool shuffleanswers, bool pausegame, Action<Popup> showncallback = null, int delay = 0, bool forcedisplay = false)
        {
            this.PopupTitle = title;
            this.PopupText = text;
            this.Answers = answers;
            this.PauseGame = pausegame;
            this.ForceDisplay = forcedisplay;
            this.ShuffleAnswers = shuffleanswers;
            this.ShownCallback = showncallback;
            this.Delay = delay;
            showEnterConfirmation = false;
            addAnswersToText();
        }

        public Popup(string title, List<string> lines, List<string> answers, bool shuffleanswers, bool pausegame, bool showconfirmation, Action<Popup> showncallback = null, int delay = 0, bool forcedisplay = false)
        {
            this.PopupTitle = title;
            this.popupLines = lines;
            this.Answers = answers;
            this.PauseGame = pausegame;
            this.showEnterConfirmation = showconfirmation;
            this.ForceDisplay = forcedisplay;
            this.ShownCallback = showncallback;
            this.Delay = delay;
            addAnswersToText();
        }

        private void addAnswersToText()
        {
            if (Answers != null)
            {
                if (ShuffleAnswers)
                {
                    answersAsDisplayed = Answers.Shuffle();
                }
                else
                {
                    answersAsDisplayed = new List<string>(Answers);
                }
                for (int i = 0; i < Answers.Count; i++)
                {
                    popupLines.AddRange(("[" + (i + 1).ToString() + "] " + Answers[i]).WrapText(720, "Arial Bold", 15.0f, out PopupTextLineHeight));
                }
            }
        }

        public void Display()
        {
            if (!cleanGameFibersRunning)
            {
                cleanGameFibers();
            }
            hasDisplayed = false;
            IndexOfGivenAnswer = -1;
            popupQueue.Add(this);
            if (fiber != null && popupFibersToDelete.Contains(fiber))
            {
                popupFibersToDelete.Remove(fiber);
            }
            Game.LogTrivial("Adding " + PopupTitle + " popup to queue.");
            fiber = new GameFiber(delegate
            {
                
                while (!ForceDisplay)
                {
                    GameFiber.Yield();
                    if (!CommonVariables.DisplayTime && !Game.IsPaused)
                    {
                        if (popupQueue.Count > 0 && popupQueue[0] == this)
                        {
                            break;
                        }
                        else if (popupQueue.Count == 0)
                        {
                            break;
                        }
                    }
                }

                CommonVariables.DisplayTime = true;
                if (PauseGame)
                {
                    Game.IsPaused = true;
                }
                if (showEnterConfirmation)
                {
                    popupLines.AddRange(("Press Enter to close.").WrapText(720, "Arial Bold", 15.0f, out PopupTextLineHeight));
                }
                isDisplaying = true;
                GameFiber.Sleep(Delay);
                popupQueue.Remove(this);
                Game.RawFrameRender += DrawPopup;
                Game.LogTrivial("Drawing " + PopupTitle + " popup message");

                timer.Restart();
                if (showEnterConfirmation)
                {
                    while (isDisplaying)
                    {
                        if (PauseGame)
                        {
                            Game.IsPaused = true;
                        }
                        GameFiber.Yield();
                        if (timer.ElapsedMilliseconds > 25000)
                        {
                            Game.DisplayNotification("A textbox is currently being shown in the centre of your screen. If you can't see it, RPH had an issue initializing with DirectX and your RPH console won't work either - ask for support on the RPH Discord (link at www.ragepluginhook.net");
                            timer.Restart();
                        }
                        if (Game.IsKeyDown(Keys.Enter))
                        {
                            Game.LogTrivial("ClosePopup is pressed");
                            Hide();
                            break;
                        }
                    }
                }

                else if (Answers != null && Answers.Count > 0)
                {
                    while (isDisplaying)
                    {
                        if (PauseGame)
                        {
                            Game.IsPaused = true;
                        }
                        GameFiber.Yield();
                        if (timer.ElapsedMilliseconds > 25000)
                        {
                            Game.DisplayNotification("A textbox is currently being shown in the centre of your screen. If you can't see it, RPH had an issue initializing with DirectX and your RPH console won't work either - ask for support on the RPH Discord (link at www.ragepluginhook.net");
                            timer.Restart();
                        }

                        if (Game.IsKeyDown(Keys.D1))
                        {
                            if (answersAsDisplayed.Count >= 1)
                            {
                                IndexOfGivenAnswer = Answers.IndexOf(answersAsDisplayed[0]);
                                Hide();

                            }
                        }
                        if (Game.IsKeyDown(Keys.D2))
                        {
                            if (answersAsDisplayed.Count >= 2)
                            {
                                IndexOfGivenAnswer = Answers.IndexOf(answersAsDisplayed[1]);
                                Hide();
                            }
                        }
                        if (Game.IsKeyDown(Keys.D3))
                        {
                            if (answersAsDisplayed.Count >= 3)
                            {
                                IndexOfGivenAnswer = Answers.IndexOf(answersAsDisplayed[2]);
                                Hide();
                            }
                        }
                        if (Game.IsKeyDown(Keys.D4))
                        {
                            if (answersAsDisplayed.Count >= 4)
                            {
                                IndexOfGivenAnswer = Answers.IndexOf(answersAsDisplayed[3]);
                                Hide();
                            }
                        }
                        if (Game.IsKeyDown(Keys.D5))
                        {
                            if (answersAsDisplayed.Count >= 5)
                            {
                                IndexOfGivenAnswer = Answers.IndexOf(answersAsDisplayed[4]);
                                Hide();
                            }
                        }
                        if (Game.IsKeyDown(Keys.D6))
                        {
                            if (answersAsDisplayed.Count >= 6)
                            {
                                IndexOfGivenAnswer = Answers.IndexOf(answersAsDisplayed[5]);
                                Hide();
                            }
                        }
                    }
                }
                timer.Stop();
            });
            fiber.Start();
            
            
        }

        public void Hide()
        {
            if (isDisplaying)
            {
                Game.RawFrameRender -= DrawPopup;
                CommonVariables.DisplayTime = false;
                
                if (PauseGame)
                {
                    Game.IsPaused = false;
                }
                isDisplaying = false;
                hasDisplayed = true;
                popupFibersToDelete.Add(fiber);
                if (ShownCallback != null)
                {
                    ShownCallback(this);
                }
                
            }
        }

        private void DrawPopup(System.Object sender, Rage.GraphicsEventArgs e)
        {
            if (isDisplaying)
            {
                Rectangle drawRect = new Rectangle(Game.Resolution.Width / 4, Game.Resolution.Height / 7, 750, 200);
                Rectangle drawBorder = new Rectangle(Game.Resolution.Width / 4 - 5, Game.Resolution.Height / 7 - 5, 760, 210);
                e.Graphics.DrawRectangle(drawBorder, Color.FromArgb(90, Color.Black));
                e.Graphics.DrawRectangle(drawRect, Color.Black);

                e.Graphics.DrawText(PopupTitle, "Aharoni Bold", 18.0f, new PointF(drawBorder.X + 5, drawBorder.Y + 5), Color.White, drawBorder);
                double LineModifier = 0;
                foreach (string line in popupLines)
                {
                    e.Graphics.DrawText(line, "Arial Bold", 15.0f, new PointF(drawRect.X, (float)(drawRect.Y + 35 + LineModifier)), Color.White, drawRect);
                    LineModifier += PopupTextLineHeight + 2;
                }
            }
        }
    }
}
