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

    var Tab = function(content){
        const {name, className, style} = content || {};
        return new Tab.init(name, className, style);
    };

    Tab.init = function(name, className, style){
        var self = this;
        self.name = name;
        self.className = className;
        self.style = style;
    };

    Tab.prototype = {
        getTab: function(selector)
        {
            var self = this;
            var root_a = document.createElement("a");
            var icon = document.createElement("i");
            var child_span = document.createElement("span");
            if (self.style !== undefined) {
                root_a.setAttribute("style", self.style);
            }
            
            icon.setAttribute("aria-hidden", "true");
            icon.setAttribute("class", self.className);
           
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