// var root_a = document.createElement("a");
// root_a.setAttribute("value", element.id);
// root_a.addEventListener("click", ChangeDashboard);
// root_a.setAttribute("style", "cursor: pointer;");
// var child_span = document.createElement("span");
// child_span.textContent = element.name;
// var root_divider = document.createElement("span");
// root_divider.setAttribute("class", "divider hidden-xs");
// root_a.appendChild(child_span);
// tab_container.appendChild(root_a);
// tab_container.appendChild(root_divider);
(function(global, $){

    var Tab = function(name, icon_tag, padding_setting){
        return new Tab.init(name, icon_tag, padding_setting);
    };

    var name;
    var icon_tag;

    Tab.init = function(name, icon_tag, padding_setting){
        var self = this;
        self.name = name;
        self.icon_tag = icon_tag;
        self.padding_setting = padding_setting;
    };

    Tab.prototype = {
        getTab: function(selector)
        {
            var self = this;
            var root_a = document.createElement("a");
            var icon = document.createElement("i");
            var child_span = document.createElement("span");

            if (self.padding_setting !== undefined) {
                root_a.setAttribute("style", self.padding_setting);
            }
            
            icon.setAttribute("aria-hidden", "true");
            icon.setAttribute("class", self.icon_tag);
           
            child_span.textContent = " " + self.name;
            
            root_a.appendChild(icon);
            root_a.appendChild(child_span);
            return root_a;
        },

        getDivider: function(selector)
        {
            var root_divider = document.createElement("span");
            root_divider.setAttribute("class", "divider hidden-xs");
            return root_divider;
        }
    };
    
    Tab.init.prototype = Tab.prototype;

    global.Tab = global.$Tab = Tab;

})(window, jQuery)