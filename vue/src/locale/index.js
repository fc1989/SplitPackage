import Vue from 'vue';
import Locales from './locale';
import zhLocale from 'iview/src/locale/lang/zh-CN';
import enLocale from 'iview/src/locale/lang/en-US';
import VueI18n from 'vue-i18n';

Vue.use(VueI18n);
const dateTimeFormats = {
  'en-US': {
    short: {
      year: 'numeric', month: 'short', day: 'numeric'
    },
    long: {
      year: 'numeric', month: 'short', day: 'numeric',
      weekday: 'short', hour: 'numeric', minute: 'numeric'
    }
  },
  'zh-Hans':{
    short: {
      year: 'numeric', month: 'short', day: 'numeric'
    },
    long: {
      year: 'numeric', month: 'short', day: 'numeric',
      weekday: 'short', hour: 'numeric', minute: 'numeric'
    }
  }
}
const messages = {
  "zh-Hans": $.extend({},zhLocale,Locales["zh-Hans"]),
  "en-US": $.extend({},enLocale,Locales["en-US"]),
}
export default new VueI18n({
    dateTimeFormats:dateTimeFormats,
    locale: 'zh-Hans',
    fallbackLocale: 'en-US',
    messages: messages
  });