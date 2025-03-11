    // 設定表單驗證規則
    $.validator.setDefaults({
        errorElement: 'span',
    errorClass: 'text-danger'
    });

    // 自訂驗證方法
    $.validator.addMethod("customValidation", function(value, element) {
        // 在這裡添加自訂的驗證邏輯
        return this.optional(element) || value.length >= 3;
    }, "請輸入至少 3 個字元");
