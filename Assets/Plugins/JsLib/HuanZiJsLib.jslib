mergeInto(LibraryManager.library, {

    //测试
    Test: function(text){
        console.log(Pointer_stringify(text));
    },

    //跳转新页面
    GoToView: function (url) {
        //新页面打开
        //window.open(Pointer_stringify(url));
        
        //当前页面打开
        window.location.href = Pointer_stringify(url);
    }
    
});