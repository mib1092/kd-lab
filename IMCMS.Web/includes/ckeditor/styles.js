/**
* Copyright (c) 2003-2015, CKSource - Frederico Knabben. All rights reserved.
* For licensing, see LICENSE.md or http://ckeditor.com/license
*/

// This file contains style definitions that can be used by CKEditor plugins.
//
// The most common use for it is the "stylescombo" plugin, which shows a combo
// in the editor toolbar, containing all styles. Other plugins instead, like
// the div plugin, use a subset of the styles on their feature.
//
// If you don't have plugins that depend on this file, you can simply ignore it.
// Otherwise it is strongly recommended to customize this file to match your
// website requirements and design properly.

CKEDITOR.stylesSet.add('default', [
	//{ name: 'Paragraph', element: 'p', styles: { 'display': 'block', 'padding': '0', 'font-family': '"Lato",sans-serif', 'font-size': '14px', 'letter-spacing': '.05em', 'line-height': '22px', 'color': '#323232' } },
	//{ name: 'Caption', element: 'p', attributes: { 'class': 'caption' }, styles: { 'display': 'block', 'margin': '0', 'padding': '0', 'font-family': '"Lato",sans-serif', 'font-size': '12px', 'letter-spacing': '.05em', 'line-height': '22px', 'color': '#8a8a8a' } },
	//{ name: 'Heading', element: 'h3', styles: { 'display': 'block', 'padding': '0', 'font-family': '"Lato",sans-serif', 'font-size': '30px', 'font-weight': '700', 'letter-spacing': '.05em', 'line-height': '35px', 'color': '#034f83', 'text-transform': 'uppercase' } },
	//{ name: 'Subheading', element: 'h4', styles: { 'display': 'block', 'padding': '0', 'font-family': '"Lato",sans-serif', 'font-size': '19px', 'font-weight': '700', 'letter-spacing': '.05em', 'line-height': '22px', 'color': '#034f83', 'text-transform': 'uppercase' } },
  //{ name: 'ReviewText', element: 'p', attributes: { 'class': 'reviewText' }, styles: { 'display': 'block', 'padding': '0', 'font-family': '"Lato",sans-serif', 'font-size': '18px', 'letter-spacing': '.05em', 'line-height': '26px', 'color': '#323232', 'background': 'yellow', 'border': '1px dashed #323232' } }
  { name: 'Heading 2', element: 'h2' },
  { name: 'Heading 3', element: 'h3' },
  { name: 'Paragraph', element: 'p' },
  //{ name: 'Caption', element: 'p', attributes: { 'class': 'caption' } },
  //{ name: 'Heading 3', element: 'h3' },
  //{ name: 'Heading 4', element: 'h4' },
  { name: 'ReviewText', element: 'p', attributes: { 'class': 'reviewText' } },
  { name: 'whiteCTA', element: 'a', attributes: { 'class': 'defaultCTA' } },
  { name: 'greenCTA', element: 'a', attributes: { 'class': 'defaultCTA02' } },
  { name: 'greyCTA', element: 'a', attributes: { 'class': 'defaultCTA03' } },
  { name: 'yellowCTA', element: 'a', attributes: { 'class': 'defaultCTA04' } },
  { name: 'yellowAltCTA', element: 'a', attributes: { 'class': 'defaultCTA04alt' } }
]);

// .defaultCTA, .defaultCTA02, .defaultCTA03, .defaultCTA04, .defaultCTA04alt