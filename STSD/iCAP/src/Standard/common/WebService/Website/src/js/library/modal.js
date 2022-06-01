export function modal(id)
{
    //var modalId;
    function show()
    {
        $('#' + id).modal("show");
    }

    function hide()
    {
        $('#' + id).modal("hide");
    }

    function toDoWhenClose(toDo)
    {
        $('#' + id).on('hidden.bs.modal', function() {
            toDo();
        });
    }

    var publicAPI = {
        show : show,
        hide : hide,
        toDoWhenClose : toDoWhenClose
    };

    return publicAPI;
}

export function modalBack()
{
    var background = document.createElement('div');
    function fade()
    {
        var main = document.getElementById('main');
    
        background.setAttribute('class', 'modal-background');
        main.insertAdjacentElement('afterend', background);
    }

    function removeFade()
    {
        background.remove();
    }

    return {
        fade: fade,
        removeFade: removeFade
    };
}