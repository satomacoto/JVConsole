using CommandLine;
using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms; // 進捗フォーム用に追加

namespace JVDownloader
{
    // エラー出力を統一するための Logger クラス
    public static class Logger
    {
        /// <summary>
        /// openStatus のエラーコードに基づく詳細なエラーメッセージを取得します。
        /// </summary>
        public static string GetOpenStatusErrorMessage(int code)
        {
            switch (code)
            {
                case -1:
                    return "該当データ無し: 指定されたパラメータに合致する新しいデータがサーバーに存在しない、または最新バージョンが公開され、ユーザーが最新バージョンのダウンロードを選択した場合。※ JVClose を呼び出して取り込み処理を終了してください。";
                case -2:
                    return "セットアップダイアログでキャンセルが押された: セットアップ用データ取り込み時にユーザーがキャンセルしたため、JVClose を呼び出して取り込み処理を終了してください。";
                case -111:
                    return "dataspec パラメータが不正: パラメータの渡し方または内容に問題がある。サンプルプログラム等を参照し、正しくパラメータがJV‑Linkに渡っているか確認。";
                case -112:
                    return "fromtime パラメータが不正 (読み出し開始ポイント時刻不正): 読み出し開始時刻の指定方法または値に誤りがある。正しい指定方法を確認。";
                case -113:
                    return "fromtime パラメータが不正 (読み出し終了ポイント時刻不正): 読み出し終了時刻の指定方法または値に問題がある。正しいパラメータ指定を確認。";
                case -114:
                    return "key パラメータが不正: key の渡し方または内容に問題がある。正しくパラメータが渡されているか確認。";
                case -115:
                    return "option パラメータが不正: option の値が誤っている可能性がある。";
                case -116:
                    return "dataspec と option の組み合わせが不正: 指定された組み合わせがサポートされていない。パラメータの組み合わせに誤りがないか確認。";
                case -201:
                    return "JVInit が行われていない: JVOpen／JVRTOpenの前にJVInitが呼ばれていない。";
                case -202:
                    return "前回のJVOpen／JVRTOpen／JVMVOpenに対してJVClose が呼ばれていない（オープン中）: 前回のオープン処理がJVCloseで終了されていない。新たにオープンする前に必ずJVCloseを実施。";
                case -211:
                    return "レジストリ内容が不正（不正に変更された）: レジストリに保存されている値に問題がある。直接変更されていないか確認し、問題なければJRA‑VANへ連絡。";
                case -301:
                    return "認証エラー: 利用キーが正しくない、または複数のマシンで同一利用キーを使用している。該当マシンのJV‑Linkをアンインストールし、再インストール後、利用キーの再発行が必要。";
                case -302:
                    return "利用キーの有効期限切れ: Data Lab.サービスの有効期限が切れている、または自動延長が停止している。サービス権の再購入が必要。（通常、配布利用キーでは発生しない）";
                case -303:
                    return "利用キーが設定されていない（空値）: 利用キーが未設定。JV‑Linkインストール直後は利用キーが空のため、必ず設定。";
                case -305:
                    return "利用規約に同意していない: 利用規約に同意していないため、蓄積系データを取得できない。利用規約同意画面で内容を確認し、同意。";
                case -401:
                    return "JV‑Link内部エラー: JV‑Link内部でエラーが発生。JRA‑VANへ連絡。";
                case -411:
                    return "サーバーエラー（HTTPステータス404 NotFound）: レジストリが直接変更されたか、Data Lab.用サーバーに問題が発生している可能性。JRA‑VANのメンテナンス状況を確認し、必要に応じ連絡。";
                case -412:
                    return "サーバーエラー（HTTPステータス403 Forbidden）: Data Lab.用サーバーに問題が発生。JRA‑VANへ連絡。";
                case -413:
                    return "サーバーエラー（HTTPステータス200,403,404以外）: 上記以外のサーバーエラー。";
                case -421:
                    return "サーバーエラー（サーバーの応答が不正）: サーバー側の応答が不正。JRA‑VANへ連絡。";
                case -431:
                    return "サーバーエラー（サーバーアプリケーション内部エラー）: サーバー内部でエラーが発生。JRA‑VANへ連絡。";
                case -501:
                    return "セットアップ処理においてスタートキット（CD/DVD-ROM）が無効: 正しいスタートキットがセットされていない。正しいスタートキットをセット。";
                case -504:
                    return "サーバーメンテナンス中: サーバーがメンテナンス中。";
                default:
                    return "不明なエラーコード: " + code;
            }
        }

        /// <summary>
        /// readStatus のエラーコードに基づく詳細なエラーメッセージを取得します。
        /// </summary>
        public static string GetReadStatusErrorMessage(int code)
        {
            switch (code)
            {
                // ※ -1 および -3 は通常処理（エラーではなく、継続すべき状態）として扱うため、ここでは対象外とします。
                case -201:
                    return "JVInit が行われていない: JVRead／JVGetsに先立って、JVInit／JVOpenを呼び出してください。";
                case -202:
                    return "前回のJVOpen／JVRTOpen／JVMVOpenに対してJVClose が呼ばれていない（オープン中）: JVOpen／JVRTOpen／JVMVOpenの呼び出し後は、必ずJVCloseを実施してください。";
                case -203:
                    return "JVOpenが行われていない: JVRead／JVGetsに先立ってJVOpenを呼び出してください。";
                case -402:
                    return "ダウンロードしたファイルが異常（サイズ＝0）: 再度JVOpenからの処理をやりなおしてください。";
                case -403:
                    return "ダウンロードしたファイルが異常（データ内容）: 再度JVOpenからの処理をやりなおしてください。";
                case -502:
                    return "ダウンロード失敗（通信エラー／ディスクエラー）: 原因を除去できたらJVCloseを呼び出し、JVOpenからの処理を再試行してください。";
                case -503:
                    return "一時ファイルが見つからない: JVOpenからJVRead／JVGetsまでの間に、使用する一時ファイルが削除された、または該当ファイルが使用中です。JVOpenから処理をやりなおす必要があります。";
                default:
                    return "不明なエラーコード: " + code;
            }
        }

        /// <summary>
        /// エラーを一貫した形式で出力します。
        /// errorSource でエラー発生元（openStatusかreadStatus）を指定できます。
        /// progressForm が渡された場合、フォームにもエラーメッセージを表示します。
        /// </summary>
        public static void LogError(string message, int errorCode, string errorSource, ProgressForm progressForm = null)
        {
            var logEntry = new
            {
                Timestamp = DateTime.Now,
                ErrorSource = errorSource,
                ErrorCode = errorCode,
                Message = message
            };
            string logOutput = $"Timestamp: {logEntry.Timestamp}, ErrorSource: {logEntry.ErrorSource}, ErrorCode: {logEntry.ErrorCode}, Message: {logEntry.Message}";
            Console.Error.WriteLine(logOutput);
            progressForm?.AppendStatus("ERROR (" + errorSource + "): " + message);
        }
    }

    // 進捗表示用フォーム
    public class ProgressForm : Form
    {
        private Label labelProgress;
        private ProgressBar progressBar;
        private TextBox textBoxStatus; // ログ出力用テキストボックス

        // デフォルトコンストラクタ
        public ProgressForm()
        {
            InitializeComponents();
        }

        // 総ファイル数を受け取るコンストラクタ（プログレスバーの最大値設定）
        public ProgressForm(int totalCount) : this()
        {
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Minimum = 0;
            progressBar.Maximum = totalCount;
            labelProgress.Text = $"Processed files: 0 / {totalCount}";
        }

        private void InitializeComponents()
        {
            this.Text = "進捗状況";
            this.Width = 400;
            this.Height = 300;
            this.StartPosition = FormStartPosition.CenterScreen;

            labelProgress = new Label();
            labelProgress.AutoSize = true;
            labelProgress.Location = new System.Drawing.Point(20, 20);
            labelProgress.Text = "処理中...";
            this.Controls.Add(labelProgress);

            progressBar = new ProgressBar();
            progressBar.Location = new System.Drawing.Point(20, 50);
            progressBar.Width = 340;
            this.Controls.Add(progressBar);

            textBoxStatus = new TextBox();
            textBoxStatus.Multiline = true;
            textBoxStatus.ReadOnly = true;
            textBoxStatus.ScrollBars = ScrollBars.Vertical;
            textBoxStatus.Location = new System.Drawing.Point(20, 90);
            textBoxStatus.Width = 340;
            textBoxStatus.Height = 150;
            this.Controls.Add(textBoxStatus);
        }

        // 現在の進捗値を更新する（プログレスバーとラベル）
        public void UpdateProgress(int currentCount)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateProgress(currentCount)));
            }
            else
            {
                progressBar.Value = Math.Min(currentCount, progressBar.Maximum);
                labelProgress.Text = $"Processed files: {currentCount} / {progressBar.Maximum}";
            }
        }

        // ステータスメッセージをテキストボックスに追記する
        public void AppendStatus(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => AppendStatus(message)));
            }
            else
            {
                textBoxStatus.AppendText($"{DateTime.Now:HH:mm:ss} - {message}{Environment.NewLine}");
            }
        }
    }

    internal class Program
    {
        const string Sid = "UNKNOWN";

        [Verb("setup", HelpText = "設定ダイアログの表示")]
        public class SetupOptions { }

        [Verb("jv", HelpText = "蓄積系データのダウンロード")]
        public class JvOptions
        {
            [Option("dataspec", Required = true, Separator = ',', HelpText = @"dataspec. see http://jra-van.jp/dlb/sdv/sdk.html, http://jra-van.jp/dlb/sdv/sdk/JV-Data470.pdf pp.47-48

option = 1
TOKU,RACE,DIFN,BLDN,SNPN,SLOP,WOOD,YSCH,HOSN,HOYU,COMM,MING

option = 2
TOKU,RACE,TCVN,RCVN,SNPN

option = 3,4
TOKU,RACE,DIFN,BLDN,SNPN,SLOP,WOOD,YSCH,HOSN,HOYU,COMM,MING")]
            public IEnumerable<string> Dataspec { get; set; }

            [Option("fromdate", Required = false, Default = "20211101000000", HelpText = @"fromdate. YYYYMMDDhhmmss or YYYYMMDDhhmmss-YYYYMMDDhhmmss.
e.g. 20181001000000")]
            public string Fromdate { get; set; }

            [Option("option", Required = true, HelpText = "1:通常データ, 2:今週データ, 3:セットアップデータ, 4:ダイアログ無しセットアップデータ")]
            public int Option { get; set; }

            [Option("outputDir", Required = false, Default = ".", HelpText = @"output directory")]
            public string OutputDir { get; set; }

            [Option("wait", Required = false, Default = 1000, HelpText = @"処理間隔 [sec]")]
            public int Wait { get; set; }
        }

        [Verb("jvrt", HelpText = "速報系データのダウンロード")]
        public class JvrtOptions
        {
            [Option("dataspec", Required = true, Separator = ',', HelpText = @"dataspec. see http://jra-van.jp/dlb/sdv/sdk.html, http://jra-van.jp/dlb/sdv/sdk/JV-Data470.pdf pp.47-48

0B12 速報レース情報（成績確定後） 開催日単位またはレース毎
0B15 速報レース情報（出走馬名表～） 開催日単位またはレース毎
0B30 速報オッズ（全賭式） レース毎
0B31 速報オッズ（単複枠） レース毎
0B32 速報オッズ（馬連） レース毎
0B33 速報オッズ（ワイド） レース毎
0B34 速報オッズ（馬単） レース毎
0B35 速報オッズ（３連複） レース毎
0B36 速報オッズ（３連単） レース毎
0B20 速報票数（全賭式） レース毎
0B11 速報馬体重 開催日単位またはレース毎
0B14 速報開催情報（一括） 開催日単位
0B16 速報開催情報（指定） 指定された変更情報単位
0B13 速報タイム型データマイニング予想 開催日単位またはレース毎
0B17 速報対戦型データマイニング予想 開催日単位またはレース毎
0B41 時系列オッズ（単複枠） レース毎（該当レースの複数時間帯のオッズ）
0B42 時系列オッズ（馬連） レース毎（該当レースの複数時間帯のオッズ）
0B51 速報重勝式(WIN5) 重勝式開催毎")]
            public IEnumerable<string> Dataspec { get; set; }

            [Option("key", Required = true, Separator = ',', HelpText = @"該当データを取得するための要求キー
カンマ区切りで複数指定
レース毎の場合 `YYYYMMDDJJKKHHRR` または `YYYYMMDDJJRR`
開催日単位の場合 `YYYYMMDD`
YYYY:開催年, MM:開催月, DD:開催日, JJ:場コード, KK:回次, HH:日次, RR:レース番号

場コード: 01札,02函,03福,04新,05東,06中,07名,08京,09阪,10小")]
            public IEnumerable<string> Key { get; set; }

            [Option("outputDir", Required = false, Default = ".", HelpText = @"output directory")]
            public string OutputDir { get; set; }

            [Option("wait", Required = false, Default = 1000, HelpText = @"処理間隔 [sec]")]
            public int Wait { get; set; }
        }

        [STAThread]
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<SetupOptions, JvOptions, JvrtOptions>(args)
                .WithParsed<SetupOptions>(RunSetupOptions)
                .WithParsed<JvOptions>(RunJvOptions)
                .WithParsed<JvrtOptions>(RunJvrtOptions)
                .WithNotParsed(HandleParseError);
        }

        static void RunSetupOptions(SetupOptions opts)
        {
            var jvLink = new JVDTLabLib.JVLink();
            jvLink.JVSetUIProperties();
            jvLink.JVClose();
        }

        static void RunJvOptions(JvOptions opts)
        {
            if (string.IsNullOrWhiteSpace(opts.Fromdate))
            {
                throw new Exception("fromdateを指定してください");
            }
            if (opts.Option < 1 || opts.Option > 4)
            {
                throw new Exception("optionは1,2,3,4のいずれかを指定してください");
            }

            // ここでのステータスは progressForm を通じて表示するため、RunJV 内で更新
            var jvLink = new JVDTLabLib.JVLink();
            jvLink.JVInit(Sid);
            foreach (var dataspec in opts.Dataspec)
            {
                RunJV(jvLink, dataspec, opts.Fromdate, opts.Option, opts.OutputDir, opts.Wait);
            }
        }

        static void RunJvrtOptions(JvrtOptions opts)
        {
            var jvLink = new JVDTLabLib.JVLink();
            jvLink.JVInit(Sid);
            foreach (var dataspec in opts.Dataspec)
            {
                foreach (var key in opts.Key)
                {
                    RunJVRT(jvLink, dataspec, key, opts.OutputDir, opts.Wait);
                }
            }
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            // エラー処理
        }

        // JVDownload処理内に進捗フォームを組み込んだ例（JV用）
        static void RunJV(JVDTLabLib.JVLink jvLink, string dataspec, string fromdate, int option, string outputDir, int wait = 1000)
        {
            var nReadCount = 0;             // JVOpen: 総読み込みファイル数
            var nDownloadCount = 0;         // JVOpen: 総ダウンロードファイル数
            var strLastFileTimestamp = "";  // JVOpen: 最新ファイルのタイムスタンプ

            var openStatus = jvLink.JVOpen(dataspec, fromdate, option, ref nReadCount, ref nDownloadCount, out strLastFileTimestamp);

            if (openStatus != 0)
            {
                // openStatus のエラーとして、コード表に基づく詳細メッセージをログ出力
                Logger.LogError(Logger.GetOpenStatusErrorMessage(openStatus), openStatus, "openStatus");
                return;
            }

            // プログレスフォーム作成（総ファイル数 nReadCount を最大値に設定）
            ProgressForm progressForm = null;
            Thread progressThread = new Thread(() =>
            {
                progressForm = new ProgressForm(nReadCount);
                Application.Run(progressForm);
            });
            progressThread.SetApartmentState(ApartmentState.STA);
            progressThread.Start();
            // フォームが表示されるまで待機
            while (progressForm == null || !progressForm.IsHandleCreated)
            {
                Thread.Sleep(100);
            }

            // ステータス情報をフォームへ出力
            progressForm.AppendStatus("Data spec: " + dataspec);
            progressForm.AppendStatus("Total read count: " + nReadCount);

            var outputPath = Path.Combine(outputDir, "JV-" + dataspec + "-" + fromdate + "-" + strLastFileTimestamp + ".txt");
            var streamWriter = new StreamWriter(outputPath, false, System.Text.Encoding.UTF8);
            streamWriter.WriteLine("JV DATASPEC:" + dataspec + " FROMDATE:" + fromdate + " LASTFILETIMESTAMP:" + strLastFileTimestamp);

            // JVReadToTxt 内で進捗情報を更新（例：現在の読み込み回数）
            JVReadToTxt(jvLink, streamWriter, progressForm);

            // 読み込み完了後、フォームに完了メッセージを表示
            progressForm.AppendStatus("Finished JVReadToTxt.");
            progressForm.AppendStatus("Output file: " + outputPath);

            // 進捗フォームを閉じる
            if (progressForm.InvokeRequired)
            {
                progressForm.Invoke(new Action(() => progressForm.Close()));
            }
            else
            {
                progressForm.Close();
            }
            progressThread.Join();

            Console.WriteLine(outputPath);

            streamWriter.Close();
            jvLink.JVClose();

            Thread.Sleep(wait);
        }

        // JVRT用（必要に応じてこちらもフォーム出力に変更可能）
        static void RunJVRT(JVDTLabLib.JVLink jvLink, string dataspec, string key, string outputDir, int wait = 1000)
        {
            var openStatus = jvLink.JVRTOpen(dataspec, key);

            if (openStatus != 0)
            {
                // openStatus のエラーとして、コード表に基づく詳細メッセージをログ出力
                Logger.LogError(Logger.GetOpenStatusErrorMessage(openStatus), openStatus, "openStatus");
                return;
            }

            var outputPath = Path.Combine(outputDir, "JVRT-" + dataspec + "-" + key + ".txt");
            var streamWriter = new StreamWriter(outputPath, false, System.Text.Encoding.UTF8);
            streamWriter.WriteLine("JVRT DATASPEC:" + dataspec + " KEY:" + key);

            // JVRTの場合、ここではフォーム出力は行わず、通常通り処理
            JVReadToTxt(jvLink, streamWriter);

            Console.WriteLine(outputPath);

            streamWriter.Close();
            jvLink.JVClose();

            Thread.Sleep(wait);
        }

        // JVReadToTxt 内で進捗フォームがあれば読み込み状況を更新
        static void JVReadToTxt(JVDTLabLib.JVLink jvLink, StreamWriter streamWriter, ProgressForm progressForm = null)
        {
            var nBuffSize = 110000;                         // JVRead: データ格納バッファサイズ
            var nNameSize = 256;                            // JVRead: ファイル名サイズ
            var strBuff = new string('\0', nBuffSize);      // JVRead: データ格納バッファ
            var strFileName = new string('\0', nNameSize);  // JVRead: 読み込み中ファイル名

            var errorMessage = "";

            bool flg_exit = false;
            int currentReadCount = 0;
            int readStatus = 0;
            do
            {
                readStatus = jvLink.JVRead(out strBuff, out nBuffSize, out strFileName);

                // 進捗フォームがある場合、現在の読み込み件数を更新し、ステータスを出力
                progressForm?.UpdateProgress(currentReadCount);

                switch (readStatus)
                {
                    case 0: // 全ファイル読み込み終了
                        flg_exit = true;
                        break;
                    case -1: // ファイル切り替わり
                        currentReadCount++;
                        break;
                    case -3: // ファイルダウンロード中
                        // 少し待ってから再試行（エラー出力は行わず、継続）
                        Thread.Sleep(100);
                        break;
                    case -201: // JVInit が行われていない
                        errorMessage = Logger.GetReadStatusErrorMessage(readStatus);
                        flg_exit = true;
                        break;
                    case -202: // 前回のJVOpen／JVRTOpen／JVMVOpenに対してJVClose が呼ばれていない（オープン中）
                        errorMessage = Logger.GetReadStatusErrorMessage(readStatus);
                        flg_exit = true;
                        break;
                    case -203: // JVOpen が行われていない
                        errorMessage = Logger.GetReadStatusErrorMessage(readStatus);
                        flg_exit = true;
                        break;
                    case -402: // ダウンロードしたファイルが異常（サイズ＝0）
                    case -403: // ダウンロードしたファイルが異常（データ内容）
                        {
                            // 削除処理の結果を付加
                            int flg_delete = jvLink.JVFiledelete(strFileName);
                            string deletionResult = (flg_delete == 0) ? "削除に成功しました。" : "削除に失敗しました。";
                            errorMessage = Logger.GetReadStatusErrorMessage(readStatus) + " " + deletionResult;
                            flg_exit = true;
                        }
                        break;
                    case -502: // ダウンロード失敗（通信エラー／ディスクエラー）
                        errorMessage = Logger.GetReadStatusErrorMessage(readStatus);
                        flg_exit = true;
                        break;
                    case -503: // 一時ファイルが見つからない
                        errorMessage = Logger.GetReadStatusErrorMessage(readStatus);
                        flg_exit = true;
                        break;
                    case int ret when ret > 0:
                        streamWriter.Write(strBuff);
                        break;
                }
            }
            while (!flg_exit);

            // エラーメッセージはコンソール出力のままとする
            if (!string.IsNullOrEmpty(errorMessage))
            {
                // readStatus のエラーとしてログ出力
                Logger.LogError(errorMessage, readStatus, "readStatus", progressForm);
            }

            progressForm?.AppendStatus("Finished JVReadToTxt.");
        }
    }
}
