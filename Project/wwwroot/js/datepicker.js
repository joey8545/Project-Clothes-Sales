//// 初始化日期選擇器 114/01/06 
//$(document).ready(function () {
//    $('.datepicker').datepicker({
//        format: 'yyyy-mm-dd', // 設定日期格式為 年-月-日
//        language: 'zh-TW', // 設定語言為繁體中文
//        autoclose: true, // 選擇日期後自動關閉日期選擇器
//        todayHighlight: true, // 高亮顯示今天的日期
//        clearBtn: true, // 顯示清除按鈕
//        orientation: "bottom auto",  // 設定日期選擇器的顯示方向
//        startDate: '1990-01-01', // 設定可選擇的最早日期
//        endDate: '2100-12-31' // 設定可選擇的最晚日期
//    });
//});
// 中文：當 DOM 完全載入後執行 
$(document).ready(function () {   
    $('.datepicker').datepicker({   //初始化所有帶有 .datepicker 類別的元素
        format: 'yyyy-mm-dd',       //設定日期格式為「年-月-日」
        language: 'zh-TW',          //設定語系為繁體中文（需引入對應語系檔）
        autoclose: false,           //關閉自動關閉功能     
        todayHighlight: true,       //高亮顯示今天      
        clearBtn: true,             //顯示清除按鈕      
        orientation: "bottom auto", //日期選擇器顯示於輸入框下方，自動調整方向   
        startDate: '1990-01-01',    //設定最早可選擇日期   
        endDate: '2100-12-31',      //設定最晚可選擇日期     
        startView: 2                //一開始顯示年份視圖     
    })    
        .on('changeDate', function (e) {// 當日期改變時，觸發 change 事件，讓驗證能正確檢查欄位值
            $(this).trigger('change');
        })
        .on('show', function (e) { //當日期選擇器顯示時執行                      
            const $input = $(this); //取得目前操作的輸入框元素                      
            const dpData = $input.data('datepicker');  //根據不同版本，嘗試取得正確的內部容器（picker）
            const $dpPicker = dpData && (dpData.picker || dpData.dpDiv || dpData.widget);
            if (!$dpPicker) {  //若無法取得容器，則輸出錯誤並停止                           
                console.error('無法取得日期選擇器容器'); //輸出錯誤訊息                                
                return;
            }
            //綁定年份點擊事件 (針對年份視圖中的 .datepicker-years .year)         
            $dpPicker.off('click.customYear').on('click.customYear', '.datepicker-years .year', function () {
                const year = parseInt($(this).text(), 10);//取得點擊元素的年份文字並轉換為整數                 
                $input.val(year);  //更新輸入框僅顯示年份（例如 "2023"）                      
            });
            //綁定月份點擊事件 (針對月份視圖中的 .datepicker-months .month)      
            $dpPicker.off('click.customMonth').on('click.customMonth', '.datepicker-months .month', function () {
                const currentVal = $input.val();//取得當前輸入框的值，預期為 "yyyy"                    
                if (!currentVal || isNaN(parseInt(currentVal, 10))) return; // 中文：若輸入框值無效則中止                       
                const year = parseInt(currentVal, 10); //將輸入框值轉換成整數年份                                            
                let monthIndex = $(this).data('month'); //嘗試從 data-month 屬性取得月份索引，若不存在則用元素的 index() 
                if (typeof monthIndex === 'undefined') {//若 data-month 未定義則                                        
                    monthIndex = $(this).index(); //使用元素索引                        
                }
                const month = monthIndex + 1;  //將索引轉換為實際月份 (1~12)                      
                const mm = String(month).padStart(2, '0');  //補零至兩位數，例如 "07"                               
                $input.val(`${year}-${mm}`); //更新輸入框為「年-月」格式（例如 "2023-07"）                                               
            });
            //綁定日期點擊事件 (針對日期視圖中非 .old 與非 .new 的 td.day 元素)     
            $dpPicker.off('click.customDay').on('click.customDay', '.datepicker-days td.day:not(.old):not(.new)', function () {
                const currentVal = $input.val(); //取得當前輸入框的值，預期為 "yyyy-mm"                              
                if (!currentVal || !currentVal.includes('-')) return;  //若格式不正確則中止                             
                const parts = currentVal.split('-');  //以 "-" 分割，得到 [year, month]                            
                const year = parts[0];    //取出年份                        
                let month = parts[1];     //取出月份                         
                if (month.length === 1) { //若月份為單位數則補零                   
                    month = '0' + month;  //補零                                
                }
                const day = parseInt($(this).text(), 10); //取得點擊元素的日並轉換為整數                               
                const dd = String(day).padStart(2, '0');//補零至兩位數 (例如 "09")                           
                $input.val(`${year}-${month}-${dd}`); //更新輸入框為「年-月-日」格式（例如 "2023-07-14"）                                       
                $input.datepicker('hide'); // 選取日期後自動關閉日期選擇器
            });
        })
        .on('hide', function (e) {//當日期選擇器隱藏          
            const $input = $(this);//取得目前操作的輸入框元素時執行                     
            const dpData = $input.data('datepicker'); //取得日期選擇器相關資料             
            const $dpPicker = dpData && (dpData.picker || dpData.dpDiv || dpData.widget);   //取得內部容器              
            if ($dpPicker) {//解除綁定自定義點擊事件，避免重複綁定                         
                $dpPicker.off('click.customYear click.customMonth click.customDay');
            }
        });
    });

