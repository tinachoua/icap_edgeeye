var color_1 = 'rgb(199,51,60)';
var color_2 = 'rgb(255,226,0)';
var color_3 = 'rgb(0,159,168)';
var color_4 = 'rgb(76,171,173)';
var color_5 = 'rgba(142,191,195)';
var color_6 = 'rgba(196,220,225)';
var color_7 = 'rgba(132,132,132)';

function makeColorsWithoutRed(count) {
    switch (count) {
        case 1:
            return [color_3];
        case 2:
            return [ color_3, color_4];//
        case 3:
            return [ color_3, color_4, color_5];
        case 4:
            return [ color_3, color_4, color_5, color_6];
        case 5:
            return [ color_3, color_4, color_5, color_6, color_7];
        case 6:
            return [ color_3, color_3, color_4, color_5, color_6, color_7];
        default:
            var color = [];
            for (var i = 0; i < count; i++) {
                color[i] = color_3;
            }
            return color;
    }
}

function colors(count) {
    switch (count) {
        case 1:
            return [color_1];
        case 2:
            return [color_1, color_3];
        case 3:
            return [color_1, color_2, color_3];//
        case 4:
            return [color_1, color_2, color_3, color_6];
        case 5:
            return [color_1, color_2, color_3, color_4, color_5];
        case 6:
            return [color_1, color_2, color_3, color_4, color_5, color_6];
        case 7:
            return [color_1, color_2, color_3, color_4, color_5, color_6, color_7];
        default:
            var color = [];
            for (var i = 0; i < count; i++) {
                color[i] = color_6;
            }
            return color;
    }
}

function getWarningColor() {
    return 'rgb(199,51,60)';
}

export { makeColorsWithoutRed, colors, getWarningColor };