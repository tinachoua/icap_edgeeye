export function convertUnixTime(unix_timestamp: string) {
    const date = new Date(unix_timestamp);
    let result = [];

    const local = date.toLocaleString("zh-TW", {
        hour12: false,
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit",
        timeZone: "Asia/Taipei"
    });
    result = local.split(" ");

    return result[1];
}