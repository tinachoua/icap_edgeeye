import {
    CHART_BASE_COLOR_A,
    CHART_BASE_COLOR_B,
    CHART_BASE_COLOR_C,
    CHART_BASE_COLOR_D,
    CHART_BASE_COLOR_E,
    CHART_BASE_COLOR_F,
    HIGHLIGHT_COLOR,
    DEFAULT_COLOR
} from "../constants/globalVariable"

const DATA_2_COLORS = [
    HIGHLIGHT_COLOR,
    CHART_BASE_COLOR_A,
];
const DATA_3_COLORS = [
    HIGHLIGHT_COLOR,
    CHART_BASE_COLOR_B,
    CHART_BASE_COLOR_C,
];
const DATA_4_COLORS = [
    HIGHLIGHT_COLOR,
    CHART_BASE_COLOR_A,
    CHART_BASE_COLOR_B,
    CHART_BASE_COLOR_E,
];
const DATA_5_COLORS = [
    HIGHLIGHT_COLOR,
    CHART_BASE_COLOR_A,
    CHART_BASE_COLOR_C,
    CHART_BASE_COLOR_D,
    CHART_BASE_COLOR_E,
];
const DATA_6_COLORS = [
    HIGHLIGHT_COLOR,
    CHART_BASE_COLOR_A,
    CHART_BASE_COLOR_B,
    CHART_BASE_COLOR_C,
    CHART_BASE_COLOR_D,
    CHART_BASE_COLOR_E,
];
const DATA_7_COLORS = [
    HIGHLIGHT_COLOR,
    CHART_BASE_COLOR_A,
    CHART_BASE_COLOR_B,
    CHART_BASE_COLOR_C,
    CHART_BASE_COLOR_D,
    CHART_BASE_COLOR_E,
    CHART_BASE_COLOR_F,
];

function makeColorsWithoutHeightlight(count) {
    switch (count) {
        case 1:
            return [DEFAULT_COLOR];
        case 2:
            return DATA_3_COLORS.slice(1);
        case 3:
            return DATA_4_COLORS.slice(1);
        case 4:
            return DATA_5_COLORS.slice(1);
        case 5:
            return DATA_6_COLORS.slice(1);
        case 6:
            return DATA_7_COLORS.slice(1);
        default:
            var color = [];
            for (var i = 0; i < count; i++) {
                color[i] = DEFAULT_COLOR;
            }
            return color;
    }
}

function colors(count) {
    switch (count) {
        case 2:
            return DATA_2_COLORS;
        case 3:
            return DATA_3_COLORS
        case 4:
            return DATA_4_COLORS
        case 5:
            return DATA_5_COLORS
        case 6:
            return DATA_6_COLORS
        case 7:
            return DATA_7_COLORS
        default:
            var color = [];
            for (var i = 0; i < count; i++) {
                color[i] = DEFAULT_COLOR;
            }
            return color;
    }
}

function getHeightLightColor() {
    return HIGHLIGHT_COLOR;
}

export { 
    makeColorsWithoutHeightlight,
    colors,
    getHeightLightColor,
};