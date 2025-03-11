const chatRoom = document.querySelector("#chatRoom");
//const userName = document.querySelector("#userName");
const txtMsg = document.querySelector("#txtMSG"); //SR用
//const room = document.querySelector("#room");
const send = document.querySelector("#send"); //SR用
const result = document.querySelector("#result"); //SR用

const chatApp = Vue.createApp({
    data() {
        return {
            isVisible: false,
            isBubbleVisible: false,
            isTyping: false,
            message: "",
            messages: [
                {
                    type: "received",
                    content: "您好！我是智能助理Unicorn，很高興為您服務。",
                },
                {
                    type: "received",
                    content: "請問有什麼我可以協助您的嗎？",
                },
            ],
            defaultReplies: {
                你好: "您好！很高興為您服務。",
                hi: "Hi！有什麼我可以幫您的嗎？",
                hello: "Hello！請問需要什麼協助呢？",
                謝謝: "不客氣！還有什麼我可以幫您的嗎？",
                掰掰: "再見！祝您有愉快的一天！",
                再見: "再見！如果還有問題歡迎隨時詢問。",
            },
            // 是否客服模式
            showChatButton: false,
            // 是否進入真人客服模式
            isRealCustomerVisible: false,
            // 加入此狀態來追踪是否在客服模式
            isCustomerService: false,
            // SignalR連線物件
            hubConnection: null,
            // 預設房間名稱
            roomId: "1",
            // 使用者ID，這裡需要根據實際登入用戶設置
            userId: "1",
        };
    },
    mounted() {
        // 從 sessionStorage 讀取之前儲存的聊天記錄
        const savedMessages = sessionStorage.getItem("chatMessages");
        if (savedMessages) {
            this.messages = JSON.parse(savedMessages);
            // 檢查是否之前在真人客服模式
            const wasInCustomerService = sessionStorage.getItem("isCustomerService") === "true";
            if (wasInCustomerService) {
                this.isCustomerService = true;
                this.initializeSignalRConnection();
            }
        }
        // 3秒後顯示氣泡
        setTimeout(() => {
            if (!this.isVisible) {
                console.log("setTimeout");
                // 只在聊天視窗未開啟時顯示氣泡
                this.isBubbleVisible = true;
            }
        }, 3000);

        // 修改點擊事件監聽器
        document.addEventListener("click", (e) => {
            // 檢查點擊是否發生在聊天視窗外部
            const isClickedOutside = this.$refs.chatWidget &&
                !this.$refs.chatWidget.contains(e.target) &&
                !this.$refs.chatToggle.contains(e.target) &&
                !e.target.closest('.chat-bubble');
            // 只有當聊天視窗是開啟狀態，且點擊在外部時才關閉
            if (this.isVisible && isClickedOutside) {
                this.closeChat();
            }
        });
        // 監聽消息變化
        this.$watch(
            () => this.messages.length, // 監聽 messages 陣列長度變化
            () => this.$nextTick(() => this.scrollToBottom())
        )
        // 頁面關閉前儲存聊天記錄和客服模式狀態
        window.addEventListener("beforeunload", () => {
            sessionStorage.setItem("chatMessages", JSON.stringify(this.messages));
            sessionStorage.setItem("isCustomerService", this.isCustomerService);
        });
    },
    beforeUnmount() {
        document.removeEventListener("click", this.handleOutsideClick);
    },
    methods: {
        toggleChat() {
            this.isVisible = !this.isVisible;
            const chatWidget = document.querySelector(".chat-widget");
            if (this.isVisible) {
                this.isBubbleVisible = false;
                this.isRealCustomerVisible = false;
                if (chatWidget) {
                    chatWidget.classList.add("show");
                }
                this.$nextTick(() => this.scrollToBottom());
            } else {
                if (chatWidget) {
                    chatWidget.classList.remove("show");
                }
            }
        },
        closeChat() {
            this.isVisible = false;
            // 如果在真人客服模式，關閉SignalR連線
            if (this.hubConnection && this.isCustomerService) {
                this.hubConnection.stop();
            }
            // 關閉時重置所有狀態
            this.message = "";
            this.isTyping = false;
        },
        handleOutsideClick(e) {
            const chatWidget = this.$refs.chatWidget;
            const chatToggle = this.$refs.chatToggle;
            if (this.isVisible && chatWidget && !chatWidget.contains(e.target) && !chatToggle.contains(e.target)) {
                this.closeChat();
            }
        },
        getSmartReply(message) {
            const lowerMsg = message.toLowerCase();
            if (lowerMsg.includes("會員")) {
                this.showChatButton = true;
                return "您是否需要會員相關協助？請點擊下方按鈕與真人客服聯繫。";
            }
            if (lowerMsg.includes("訂單")) {
                this.showChatButton = true;
                return "關於您的訂單，請點擊下方按鈕與客服人員聯繫。";
            }
            if (lowerMsg.includes("活動")) {
                this.showChatButton = true;
                return "關於我們的活動，請點擊下方按鈕與客服人員聯繫。";
            }
            if (lowerMsg.includes("客製")) {
                this.showChatButton = true;
                return "我們提供客製化服務，請點擊下方按鈕與客服人員聯繫。";
            }
            if (lowerMsg.includes("價格") || lowerMsg.includes("費用")) {
                this.showChatButton = true;
                return "關於價格方案，建議您可以參考我們的方案頁面，或是告訴我您的需求，我可以為您推薦最適合的方案。";
            }
            if (lowerMsg.includes("如何") || lowerMsg.includes("怎麼")) {
                this.showChatButton = true;
                return "需要更詳細了解您的問題，可以告訴我您想知道的具體內容嗎？";
            }
            if (lowerMsg.includes("聯絡") || lowerMsg.includes("客服")) {
                this.showChatButton = true;
                return "請點擊下方按鈕連接真人客服";
            }
            for (let key in this.defaultReplies) {
                if (lowerMsg.includes(key)) {
                    return this.defaultReplies[key];
                }
            }
            return "抱歉，我可能沒有完全理解您的問題。您可以換個方式描述，或直接聯繫我們的客服人員。";
        },
        async sendMessage() {
            // 檢查訊息是否為空
            if (!this.message.trim()) return;
            //console.log(this.messages);
            // 檢查是否包含"客服"關鍵字
            const isCustomerService = this.message.includes("客服");
            console.log("發送訊息:", this.messages);
            // 儲存訊息內容
            const messageContent = this.message;
            // 添加用戶訊息
            this.messages.push({
                type: "sent",
                content: messageContent,
                timestamp: new Date().toLocaleTimeString('en-US', { hour12: false })
            });
            // 立即滾動到底部
            await this.$nextTick();
            this.scrollToBottom();
            this.saveMessages();
            this.isTyping = false;
            // 判斷是否為客服模式且已建立連線
            if (this.isCustomerService && this.hubConnection) {
                try {
                    // 透過SignalR發送訊息,發送訊息到伺服器，使用userId作為user參數
                    await this.hubConnection.invoke("SendMessage", this.userId, messageContent);
                    //this.hubConnection.invoke("SendMessage", "1", "測試訊息")
                    //    .then(() => console.log("訊息發送成功"))
                    //    .catch(err => console.error("發送訊息錯誤:", err));
                }
                catch (err) {
                    // 錯誤處理
                    console.error("發送訊息錯誤:", err);
                    // 顯示錯誤訊息
                    this.messages.push({
                        type: "system",
                        content: "訊息發送失敗，請重試"
                    });
                }
            }
            else {
                // 如果不是客服模式，使用機器人回覆
                this.handleBotResponse(messageContent);
            }
            this.$nextTick(() => this.scrollToBottom());
            // 清空訊息輸入框
            this.message = "";
            // 如果是來自其他使用者的訊息，不顯示正在輸入
            this.isTyping = false;
        },
        // 新增連接真人客服方法
        async connectToRealService() {
            // alert("正在為您連接真人客服...");
            let timerInterval;
            // 顯示Sweet Alert loading畫面
            Swal.fire({
                title: "系統通知",
                html: "正在為您連接真人客服 <b></b> milliseconds.",
                timer: 2000, // 2 秒倒數
                timerProgressBar: true,// 顯示進度條
                // Alert開啟時執行
                didOpen: () => {
                    Swal.showLoading();
                    // 獲取計時器元素
                    const timer = Swal.getPopup().querySelector("b");
                    timerInterval = setInterval(() => {
                        timer.textContent = `${Swal.getTimerLeft()}`;
                    }, 100);
                },
                // Alert關閉時清除計時器
                willClose: () => {
                    clearInterval(timerInterval);
                },
            })
                .then((result) => {
                    if (result.dismiss === Swal.DismissReason.timer) {
                        // 切換到真人客服模式
                        this.isCustomerService = true;
                        sessionStorage.setItem("isCustomerService", "true");
                        this.messages.push({
                            type: "system",
                            content: "排隊進入真人客服！！"
                        })
                        this.isCustomerService = true; // 正式進入真人客服模式
                        // 強制在下一個 tick 進行滾動
                        this.$nextTick(() => {
                            this.scrollToBottom();
                        });
                        // 初始化SignalR連線
                        this.initializeSignalRConnection();
                    }
                });
        },
        startRealCustomerService() {
            this.isRealCustomerVisible = true;
            this.isVisible = false;
        },
        // 初始化SignalR連線的方法
        async initializeSignalRConnection() {
            try {
                // 建立SignalR連線配置
                this.hubConnection = new signalR.HubConnectionBuilder()
                    .withUrl(`https://localhost:7279/ChatRoom?room=${this.userId}`)
                    .withAutomaticReconnect([0, 2000, 5000, 10000, null]) // 自動重連機制
                    .configureLogging(signalR.LogLevel.Debug) // 啟用詳細日誌
                    .build();
                // 設置接收訊息的處理器
                this.hubConnection.on("UpdContent", (msg) => {
                    // 過濾空訊息
                    if (!msg.message || msg.message.trim() === "") return;
                    //console.log("收到訊息:", content);                   
                    //const roomContainer = document.querySelector("#room");
                    if (msg.user === this.userId) {
                        return;
                    }
                    else if (msg.user === "System") {
                        this.messages.push({
                            type: "system",
                            content: msg.message,
                            timestamp: msg.timestamp || new Date().toLocaleTimeString('en-US', { hour12: false })
                        });
                    }
                    else {
                        // 將訊息加入Vue數據中
                        this.messages.push({
                            type: "received",
                            content: msg.message,
                            user: msg.user,
                            timestamp: msg.timestamp || new Date().toLocaleTimeString('en-US', { hour12: false })
                        });
                    }
                    sessionStorage.setItem("chatMessages", JSON.stringify(this.messages));
                    this.scrollToBottom();
                });
                // 啟動SignalR連線
                await this.hubConnection.start();
                //console.log("Hub 連線完成");
                // 新增系統訊息到訊息列表
                this.messages.push({
                    type: "system",
                    content: "成功輪到您進入真人客服！"
                });
                // 設定為客服模式
                this.isCustomerService = true;
            }
            catch (err) {
                // 錯誤處理
                console.error("連線錯誤:", err);
                // 將錯誤訊息加入聊天室
                this.messages.push({
                    type: "system",
                    content: "無法連接到客服系統，請稍後再試"
                });
                // 顯示錯誤提示
                Swal.fire({
                    title: '錯誤',
                    text: '無法連接到客服系統，請稍後再試',
                    icon: 'error'
                });
                // 3秒後重試
                setTimeout(() => {
                    this.initializeSignalRConnection();
                }, 3000);
            }
        },
        // 處理機器人回覆的方法
        handleBotResponse(messageContent) {
            // 設定正在輸入狀態
            this.isTyping = true;
            // 延遲1秒後回覆
            setTimeout(() => {
                // 取消正在輸入狀態
                this.isTyping = false;
                // 獲取智能回覆
                const reply = this.getSmartReply(messageContent);
                // 將回覆加入訊息列表
                this.messages.push({
                    type: "received",
                    content: reply,
                    showButton: this.showChatButton
                });
                // 等待下一個更新週期後捲動到底部
                this.$nextTick(() => this.scrollToBottom());
            }, 1000);
        },
        // 儲存訊息到 sessionStorage
        saveMessages() {
            sessionStorage.setItem("chatMessages", JSON.stringify(this.messages));
            sessionStorage.setItem("isCustomerService", this.isCustomerService);
        },
        scrollToBottom() {
            this.$nextTick(() => {
                const container = this.$refs.messagesContainer;
                if (container) {
                    setTimeout(() => {
                        container.scrollTop = container.scrollHeight;
                    }, 200); // 給予一個小延遲確保 DOM 已更新
                }
            });
        },
    }
    // 將Vue應用程式掛載到指定的DOM元素
}).mount("#chatApp");