// Package Explorer Media Utilities v1.1
// Updated: 2025-11-11 - Added downloadFileFromStream function

window.createBlobUrl = (byteArray, mimeType) => {
    const bytes = new Uint8Array(byteArray);
    const blob = new Blob([bytes], { type: mimeType });
    return URL.createObjectURL(blob);
};

window.downloadFileFromStream = async (fileName, streamReference) => {
    const arrayBuffer = await streamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
};

// Log that the script is loaded for debugging
console.log('Package Explorer media.js v1.1 loaded - downloadFileFromStream available');