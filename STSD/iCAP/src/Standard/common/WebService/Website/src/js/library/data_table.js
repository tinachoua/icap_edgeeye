export function AddHead($toAttach, item) {
    item.forEach((element) => {
        var $thead = $("<th></th>");
        $thead.text(element);
        $toAttach.find("tr").append($thead);
    });
}