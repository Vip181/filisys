using System;
using System.Collections.Generic;
using System.IO;
using Sys = Cosmos.System;

namespace filesys
{
    static class Config
    {
        public const string FileName = @"0:\screen.txt"; // disque 0, racine

        // Résolutions autorisées (width;height)
        private static readonly HashSet<string> allowed = new HashSet<string>()
        {
            "320;240",
            "640;480",
            "800;600",
            "1024;768",
            "1280;720",
            "1280;768",
            "1280;1024",
            "1366;768",
            "1680;1050",
            "1920;1080",
            "1920;1200"
        };

        // dernière résolution appliquée (string "W;H") — permet de détecter changement de contenu
        private static string lastApplied = null;

        // S'assure que le VFS existe et que le fichier screen.txt est présent et valide
        public static void initialefile()
        {
            try
            {
                Sys.FileSystem.CosmosVFS fs = new Cosmos.System.FileSystem.CosmosVFS();
                Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
            }
            catch { /* ignore si déjà enregistré */ }

            try
            {
                if (!File.Exists(FileName))
                {
                    File.WriteAllText(FileName, "1920;1080");
                    lastApplied = "1920;1080";
                    return;
                }

                var data = File.ReadAllText(FileName).Trim();
                if (IsValidResolutionString(data))
                {
                    lastApplied = data;
                }
                else
                {
                    File.WriteAllText(FileName, "1920;1080");
                    lastApplied = "1920;1080";
                }
            }
            catch
            {
                // silence: la lecture ultérieure gérera le fallback
            }
        }

        public static void SaveResolution(int width, int height)
        {
            try
            {
                string data = width + ";" + height;
                File.WriteAllText(FileName, data);
            }
            catch
            {
                TryWriteDebug("Erreur sauvegarde config");
            }
        }

        public static (int width, int height) LoadResolution()
        {
            try
            {
                if (File.Exists(FileName))
                {
                    string data = File.ReadAllText(FileName).Trim();
                    var parts = data.Split(';');
                    if (parts.Length == 2 &&
                        int.TryParse(parts[0], out int w) &&
                        int.TryParse(parts[1], out int h) &&
                        allowed.Contains($"{w};{h}"))
                    {
                        lastApplied = $"{w};{h}";
                        return (w, h);
                    }
                }
            }
            catch { /* ignore */ }

            // valeur par défaut sécurisée
            lastApplied = "1920;1080";
            return (1920, 1080);
        }

        // Vérifie le contenu du fichier ; si différent du dernier appliqué, tente d'appliquer.
        public static void CheckAndApplyResolutionChange()
        {
            try
            {
                if (!File.Exists(FileName)) return;

                string data = File.ReadAllText(FileName).Trim();

                if (string.IsNullOrEmpty(data)) return;

                if (data == lastApplied) return; // pas de changement

                // tenter d'appliquer
                if (TryApplyResolutionString(data, out string reason))
                {
                    lastApplied = data;
                    TryWriteDebug($"Resolution applied: {data}");
                }
                else
                {
                    TryWriteDebug($"Resolution NOT applied ({reason}): {data}");
                }
            }
            catch (Exception ex)
            {
                TryWriteDebug("Exception in CheckAndApplyResolutionChange: " + ex.Message);
            }
        }

        // Force l'application immédiatement (util pour console)
        public static bool ForceApplyFromFile(out string message)
        {
            try
            {
                if (!File.Exists(FileName))
                {
                    message = "screen.txt missing";
                    return false;
                }

                string data = File.ReadAllText(FileName).Trim();
                if (TryApplyResolutionString(data, out string reason))
                {
                    lastApplied = data;
                    message = "Applied: " + data;
                    return true;
                }
                else
                {
                    message = "Rejected: " + reason;
                    return false;
                }
            }
            catch (Exception ex)
            {
                message = "Exception: " + ex.Message;
                return false;
            }
        }

        // Parse et applique une string "W;H" ; retourne true si ok, false + reason si non.
        private static bool TryApplyResolutionString(string s, out string reason)
        {
            reason = "invalid format";
            if (string.IsNullOrEmpty(s)) return false;

            var parts = s.Split(';');
            if (parts.Length != 2) return false;

            if (!int.TryParse(parts[0], out int newW) || !int.TryParse(parts[1], out int newH))
                return false;

            var key = $"{newW};{newH}";
            if (!allowed.Contains(key))
            {
                reason = "not allowed";
                return false;
            }

            // appliquer en sécurité : sauvegarder, destroy, init, rollback si erreur
            int prevW = ScreenManager.Width;
            int prevH = ScreenManager.Height;

            try
            {
                ScreenManager.Destroy();
                ScreenManager.Init(newW, newH);
                reason = "ok";
                return true;
            }
            catch (Exception ex)
            {
                // rollback
                try { ScreenManager.Init(prevW, prevH); } catch { /* ignore */ }
                reason = "apply failed: " + ex.Message;
                return false;
            }
        }

        private static bool IsValidResolutionString(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            var parts = s.Split(';');
            if (parts.Length != 2) return false;
            if (!int.TryParse(parts[0], out int w)) return false;
            if (!int.TryParse(parts[1], out int h)) return false;
            return allowed.Contains($"{w};{h}");
        }

        // debug minimal : écrit dans 0:\debug_resolution.txt (silencieux si impossible)
        private static void TryWriteDebug(string message)
        {
            try
            {
                File.AppendAllText(@"0:\debug_resolution.txt", message + Environment.NewLine);
            }
            catch { }
        }
    }
}